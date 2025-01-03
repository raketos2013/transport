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

                        foreach (var taskJob in _appDbContext.Task.ToList())
                        {
                            jobKey = new JobKey(taskJob.TaskId, "FManager");
                            //Перепланирование(обновление) задач
                            if (await scheduler.CheckExists(jobKey))
                            {
                                jobDetail = await scheduler.GetJobDetail(jobKey, stoppingToken);
                                if (jobDetail != null)
                                {

                                    if (jobDetail.JobDataMap.TryGetString("LastModified", out lastModifiedJob))
                                    {
                                        if (lastModifiedJob?.ToString() != taskJob.LastModified.ToString())
                                        {
                                            if (await scheduler.DeleteJob(jobKey))
                                            {
                                                if (taskJob.IsActive)
                                                {
                                                   /* jobDetail = JobBuilder.Create<JobForTask>().WithIdentity(jobKey)
                                                        .UsingJobData("JobName", taskJob.Name)
                                                        .UsingJobData("TimeBegin", taskJob.TimeBegin.ToString())
                                                        .UsingJobData("TimeEnd", taskJob.TimeEnd.ToString())
                                                        .UsingJobData("Delay", taskJob.Delay.ToString())
                                                        .UsingJobData("IsActive", taskJob.IsActive.ToString())
                                                        .UsingJobData("LastModified", taskJob.LastModified.ToString())
                                                        .Build();

                                                    jobTrigger = GetTrigger(taskJob, ref jobDetail);
                                                    await scheduler.ScheduleJob(jobDetail, jobTrigger, stoppingToken);*/
                                                }
                                            }
                                        }
                                    }
                                }

                            }
                            else
                            {
                                //Первоначальная загрузка задач
                                if (taskJob.IsActive)
                                {
                                    /*jobDetail = JobBuilder.Create<JobForTask>().WithIdentity(jobKey)
                                        .UsingJobData("JobName", taskJob.Name)
                                        .UsingJobData("TimeBegin", taskJob.TimeBegin.ToString())
                                        .UsingJobData("TimeEnd", taskJob.TimeEnd.ToString())
                                        .UsingJobData("Delay", taskJob.Delay.ToString())
                                        .UsingJobData("IsActive", taskJob.IsActive.ToString())
                                        .UsingJobData("LastModified", taskJob.LastModified.ToString())
                                        .Build();
                                    jobTrigger = GetTrigger(taskJob, ref jobDetail);

                                    await scheduler.ScheduleJob(jobDetail, jobTrigger, stoppingToken);*/
                                }
                            }
                        }

                    }
                }
            }
        }



        /*private ITrigger GetTrigger(TaskEntity taskJob, ref IJobDetail jobDetail)
        {
            ITrigger jobtrigger;


            *//*jobtrigger = TriggerBuilder.Create().ForJob(jobDetail).WithIdentity(taskJob.TaskId).WithDailyTimeIntervalSchedule
                 (

                  s =>
                  {
                      if (taskJob.Delay.Hour != 0)
                      {
                          s.WithIntervalInHours(taskJob.Delay.Hour);
                      }
                      if (taskJob.Delay.Minute != 0)
                      {
                          s.WithIntervalInMinutes(taskJob.Delay.Minute);
                      }
                      if (taskJob.Delay.Second != 0)
                      {
                          s.WithIntervalInSeconds(taskJob.Delay.Second);
                      }

                      if ((taskJob.Delay.Minute == 0 && taskJob.Delay.Hour == 0 && taskJob.Delay.Second == 0))
                      {
                          s.WithIntervalInMinutes(1);
                      }

                      s.OnEveryDay();
                      if (!taskJob.TimeBegin.Equals(new TimeOnly()))
                      {
                          s.StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(taskJob.TimeBegin.Hour, taskJob.TimeBegin.Minute));
                      }
                      if (!taskJob.TimeEnd.Equals(new TimeOnly()))
                      {
                          s.EndingDailyAt(TimeOfDay.HourAndMinuteOfDay(taskJob.TimeEnd.Hour, taskJob.TimeEnd.Minute));
                      }
                      s.InTimeZone(TimeZoneInfo.Local);
                  }
                 ).Build();*//*


            return jobtrigger;
        }*/

    }
}
