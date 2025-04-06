using FileManager.DAL;
using FileManager.Domain.Entity;
using FileManager_Server;
using Quartz;

namespace FileManager
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly ISchedulerFactory _jobFactory;
        private IScheduler scheduler;

        public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider, ISchedulerFactory jobFactory)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _jobFactory = jobFactory;
            scheduler = _jobFactory.GetScheduler().Result;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            JobKey jobKey;
            IJobDetail? jobDetail;
            ITrigger jobTrigger;
            string? lastModifiedJob;

            while (!stoppingToken.IsCancellationRequested)
            {
                if (scheduler != null && !scheduler.IsStarted)
                {
                    await scheduler.Start();
                }
                using (var scope = _serviceProvider.CreateScope())
                {
                    var _appDbContext = scope.ServiceProvider.GetService<AppDbContext>();
                    if (_appDbContext != null && scheduler != null)
                    {

                        foreach (var task in _appDbContext.Task.ToList())
                        {
                            jobKey = new JobKey(task.TaskId, "FManager");
                            //Перепланирование(обновление) задач
                            if (await scheduler.CheckExists(jobKey))
                            {
                                jobDetail = await scheduler.GetJobDetail(jobKey, stoppingToken);
                                if (jobDetail != null &&
                                    jobDetail.JobDataMap.TryGetString("LastModified", out lastModifiedJob) &&
                                    lastModifiedJob?.ToString() != task.LastModified.ToString())
                                {
                                    if (await scheduler.DeleteJob(jobKey))
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
                                            await scheduler.ScheduleJob(jobDetail, jobTrigger, stoppingToken);
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

                                    await scheduler.ScheduleJob(jobDetail, jobTrigger, stoppingToken);
                                }
                            }
                        }
                    }
                }
            }
        }



        private ITrigger GetTrigger(TaskEntity task, ref IJobDetail jobDetail)
        {
            ITrigger jobtrigger;
            jobtrigger = TriggerBuilder.Create()
                                        .ForJob(jobDetail)
                                        .WithIdentity(task.TaskId)
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
