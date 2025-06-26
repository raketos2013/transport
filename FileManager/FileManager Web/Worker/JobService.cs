using FileManager.DAL.Repositories.Implementations;
using FileManager.Domain.Entity;
using FileManager.Services.Interfaces;
using Quartz;

namespace FileManager_Web.Worker
{
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

            var tasks = taskService.GetAllTasks();
            foreach (var task in tasks)
            {
                jobKey = new JobKey(task.TaskId, "FManager");
                //Перепланирование(обновление) задач
                if (await scheduler.CheckExists(jobKey, cancellationToken))
                {
                    jobDetail = await scheduler.GetJobDetail(jobKey, cancellationToken);
                    if (jobDetail != null &&
                        jobDetail.JobDataMap.TryGetString("LastModified", out string? lastModifiedJob) &&
                        lastModifiedJob?.ToString() != task.LastModified.ToString())
                    {
                        if (await scheduler.DeleteJob(jobKey, cancellationToken))
                        {
                            if (task.IsActive)
                            {
                                jobDetail = JobBuilder.Create<JobForTask>().WithIdentity(jobKey)
                                    .UsingJobData("JobName", task.Name)
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
                            .UsingJobData("JobName", task.Name)
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
                                        .StartNow()
                                        .WithDailyTimeIntervalSchedule(s =>
                                        {
                                            //s.WithIntervalInMinutes(1);
                                            s.WithIntervalInSeconds(10);
                                            s.OnEveryDay();
                                            if (!task.TimeBegin.Equals(new TimeOnly()))
                                            {
                                                s.StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(task.TimeBegin.Hour, task.TimeBegin.Minute));
                                            }
                                            if (!task.TimeEnd.Equals(new TimeOnly()))
                                            {
                                                s.EndingDailyAt(TimeOfDay.HourAndMinuteOfDay(task.TimeEnd.Hour, task.TimeEnd.Minute));
                                            }
                                            s.InTimeZone(TimeZoneInfo.Local);
                                        }
                     ).Build();
            return jobtrigger;
        }

    
    }
}
