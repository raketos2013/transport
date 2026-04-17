using FileManager.Core.Entities;
using FileManager.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;
using NLog;
using Quartz;

namespace FileManager.Jobs;

public class JobService(ITaskService taskService,
                        ISchedulerFactory jobFactory,
                        ILogger<JobService> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var cancellationToken = context.CancellationToken;
        //logger.LogInformation("Execute JobService!!!!");
        var scheduler = jobFactory.GetScheduler().Result;
        if (scheduler == null)
        {
            logger.LogError("Error start scheduler");
            throw new Exception("Error start scheduler");
        }
        if (scheduler != null && !scheduler.IsStarted)
        {
            await scheduler.Start(cancellationToken);
        }

        JobKey jobKey;
        IJobDetail? jobDetail;
        ITrigger jobTrigger;

        var tasks = await taskService.GetAllTasks();
        foreach (var task in tasks)
        {
            if (task.TimeBegin > task.TimeEnd)
            {
                logger.LogInformation($"--- Задача {task.TaskId} не запланирована, время начала {task.TimeBegin} больше времени окончания {task.TimeEnd}");
                continue;
            }
            jobKey = new JobKey(task.TaskId, "FManager");
            //Перепланирование(обновление) задач
            if (await scheduler.CheckExists(jobKey, cancellationToken))
            {
                jobDetail = await scheduler.GetJobDetail(jobKey, cancellationToken);
                if (jobDetail != null &&
                    jobDetail.JobDataMap.TryGetString("LastModified", out string? lastModifiedJob) &&
                    lastModifiedJob?.ToString() != task.LastModified.ToString()
                    )
                {
                    //logger.LogInformation($"ReSchedule task - {task.TaskId}");
                    var isDeleteJob = await scheduler.DeleteJob(jobKey, cancellationToken);
                    if (isDeleteJob)
                    {
                        if (task.IsActive)
                        {
                            jobDetail = JobBuilder.Create<JobForTask>().WithIdentity(jobKey)
                                .UsingJobData("JobName", task.TaskId)
                                .UsingJobData("TimeBegin", task.TimeBegin.ToString())
                                .UsingJobData("TimeEnd", task.TimeEnd.ToString())
                                .UsingJobData("IsActive", task.IsActive.ToString())
                                .UsingJobData("LastModified", task.LastModified.ToString())
                                .Build();

                            jobTrigger = GetTrigger(task, ref jobDetail);
                            logger.LogInformation($"Перепланирование задачи {task.TaskId}");
                            await scheduler.ScheduleJob(jobDetail, jobTrigger, cancellationToken);
                        }
                    }
                }
            }
            else
            {
                //Первоначальная загрузка задач
                if (task.IsActive)
                {
                    //logger.LogInformation($"Schedule task - {task.TaskId}");
                    jobDetail = JobBuilder.Create<JobForTask>().WithIdentity(jobKey)
                        .UsingJobData("JobName", task.TaskId)
                        .UsingJobData("TimeBegin", task.TimeBegin.ToString())
                        .UsingJobData("TimeEnd", task.TimeEnd.ToString())
                        .UsingJobData("IsActive", task.IsActive.ToString())
                        .UsingJobData("LastModified", task.LastModified.ToString())
                        .Build();
                    jobTrigger = GetTrigger(task, ref jobDetail);
                    logger.LogInformation($"Планирование задачи {task.TaskId}");
                    await scheduler.ScheduleJob(jobDetail, jobTrigger, cancellationToken);
                }
            }
        }
    }

    private static ITrigger GetTrigger(TaskEntity task, ref IJobDetail jobDetail)
    {
        DateTimeOffset timeBeg = DateTimeOffset.Parse(task.TimeBegin.ToString());
        DateTimeOffset timeEnd = DateTimeOffset.Parse(task.TimeEnd.ToString());
        DateTimeOffset now = DateTimeOffset.Parse(DateTime.Now.ToString());
        ITrigger jobtrigger;
        if (task.TimeBegin == task.TimeEnd && 
            task.TimeEnd.Hour == 0 &&
            task.TimeEnd.Minute == 0 &&
            task.TimeEnd.Second == 0 )
        {
            jobtrigger = TriggerBuilder.Create()
                                .ForJob(jobDetail)
                                .WithIdentity(task.TaskId)
                                .WithDailyTimeIntervalSchedule(x => x
                                    .StartingDailyAt(TimeOfDay.HourMinuteAndSecondOfDay(0, 0, 0))
                                    .EndingDailyAt(TimeOfDay.HourMinuteAndSecondOfDay(23, 59, 59))
                                    .WithIntervalInSeconds(10)
                                    .WithMisfireHandlingInstructionDoNothing()
                                )
                                .Build();
        }
        else
        {
            jobtrigger = TriggerBuilder.Create()
                                        .ForJob(jobDetail)
                                        .WithIdentity(task.TaskId)
                                        .WithDailyTimeIntervalSchedule(x => x
                                            .StartingDailyAt(TimeOfDay.HourMinuteAndSecondOfDay(task.TimeBegin.Hour,
                                                                                                task.TimeBegin.Minute,
                                                                                                task.TimeBegin.Second))
                                            .EndingDailyAt(TimeOfDay.HourMinuteAndSecondOfDay(task.TimeEnd.Hour,
                                                                                                task.TimeEnd.Minute,
                                                                                                task.TimeEnd.Second))
                                            .WithIntervalInSeconds(10)
                                            .WithMisfireHandlingInstructionDoNothing()
                                        )
                                        .Build();
        }
            


        //.WithCalendarIntervalSchedule(s => s
        //    .WithIntervalInSeconds(10)
        //    .InTimeZone(TimeZoneInfo.Local)
        //)
        //.StartAt(timeBeg)
        //.EndAt(timeEnd)
        return jobtrigger;
    }
}
