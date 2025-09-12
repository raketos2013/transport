using FileManager.Core.Entities;
using FileManager.Core.Interfaces.Services;
using Quartz;

namespace FileManager.Jobs;

public class JobService(ITaskService taskService,
                        ISchedulerFactory jobFactory) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var cancellationToken = context.CancellationToken;
        Console.WriteLine("Execute JobService!!!!");
        var scheduler = jobFactory.GetScheduler().Result;
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
                    jobDetail = JobBuilder.Create<JobForTask>().WithIdentity(jobKey)
                        .UsingJobData("JobName", task.TaskId)
                        .UsingJobData("TimeBegin", task.TimeBegin.ToString())
                        .UsingJobData("TimeEnd", task.TimeEnd.ToString())
                        .UsingJobData("IsActive", task.IsActive.ToString())
                        .UsingJobData("LastModified", task.LastModified.ToString())
                        .Build();
                    jobTrigger = GetTrigger(task, ref jobDetail);

                    await scheduler.ScheduleJob(jobDetail, jobTrigger, cancellationToken);
                }
            }
        }
    }

    private static ITrigger GetTrigger(TaskEntity task, ref IJobDetail jobDetail)
    {
        ITrigger jobtrigger;
        jobtrigger = TriggerBuilder.Create()
                                    .ForJob(jobDetail)
                                    .WithIdentity(task.TaskId)
                                    .WithCalendarIntervalSchedule(s =>
                                    {
                                        s.WithIntervalInSeconds(10);
                                        s.InTimeZone(TimeZoneInfo.Local);
                                    })
                                    .StartAt(DateTimeOffset.Parse(task.TimeBegin.ToString()))
                                    .EndAt(DateTimeOffset.Parse(task.TimeEnd.ToString()))
                                    .Build();
        return jobtrigger;
    }
}
