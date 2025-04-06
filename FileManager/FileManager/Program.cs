using FileManager.DAL;
using FileManager;
using Quartz;
using FileManager_Server;
using FileManager_Server.Loggers;

var builder = Host.CreateApplicationBuilder(args);
builder.Logging.AddFile(Path.Combine(Directory.GetCurrentDirectory(), $"logger_{DateOnly.FromDateTime(DateTime.Now)}.txt"));
builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddScoped<ITaskLogger, TaskLogger>();

builder.Services.AddQuartz(q => {

	var jobKey = new JobKey(nameof(ProcessOutboxMessagesJob));

	q.AddJob<ProcessOutboxMessagesJob>(jobKey)
	 .AddTrigger(
			trigger => trigger.ForJob(jobKey).WithSimpleSchedule(
				schedule => schedule.WithIntervalInSeconds(5).RepeatForever()));

	q.UseMicrosoftDependencyInjectionJobFactory();
    
});
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

builder.Services.AddHostedService<Worker>();

var host = builder.Build();

host.RunAsync();





