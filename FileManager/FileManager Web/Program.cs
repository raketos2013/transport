using FileManager.DAL;
using FileManager.DAL.Repositories.Implementations;
using FileManager.DAL.Repositories.Interfaces;
using FileManager.Services.Implementations;
using FileManager.Services.Interfaces;
using FileManager_Web.Loggers;
using FileManager_Web.Logging;
using FileManager_Web.MailSender;
using FileManager_Web.Worker;
using Microsoft.AspNetCore.Authentication.Cookies;
using Quartz;

namespace FileManager_Web;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddQuartz(q =>
        {
            /*q.SchedulerId = "Scheduler.Core";
            q.SchedulerName = "FileManager";

            q.MaxBatchSize = 5;
            q.InterruptJobsOnShutdown = true;*/


            var jobKey = new JobKey("JobTask", "FManager");
            q.AddJob<JobService>(opts => opts.WithIdentity(jobKey));

            q.AddTrigger(opts => opts
                .ForJob(jobKey)
                .WithIdentity("Scheduling-Tasks")
                .WithCalendarIntervalSchedule(s =>
                {
                    s.WithIntervalInSeconds(15);

                    s.InTimeZone(TimeZoneInfo.Local);
                })
                .StartAt(DateTimeOffset.Parse("00:00"))
                .EndAt(DateTimeOffset.Parse("23:59"))
            );
        }
            );
        builder.Services.AddQuartzHostedService(options =>
        {
            //options.StartDelay = TimeSpan.FromMilliseconds(1_000);
            //options.AwaitApplicationStarted = true;
            options.WaitForJobsToComplete = true;
        });




        // Add services to the container.
        builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(option =>
        {
            option.LoginPath = "/Account/Login";
            option.AccessDeniedPath = "/Account/Login";
            option.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        });

        builder.Services.AddDbContext<AppDbContext>();

        builder.Services.AddHttpContextAccessor();

        builder.Services.AddScoped<ITaskRepository, TaskRepository>();
        builder.Services.AddScoped<IStepRepository, StepRepository>();
        builder.Services.AddScoped<IAddresseeRepository, AddresseeRepository>();
        builder.Services.AddScoped<IOperationRepository, OperationRepository>();
        builder.Services.AddScoped<ITaskLogRepository, TaskLogRepository>();
        builder.Services.AddScoped<IUserLogRepository, UserLogRepository>();

        builder.Services.AddScoped<ITaskService, TaskService>();
        builder.Services.AddScoped<IStepService, StepService>();
        builder.Services.AddScoped<IAddresseeService, AddresseeService>();
        builder.Services.AddScoped<IOperationService, OperationService>();
        builder.Services.AddScoped<ITaskLogService, TaskLogService>();
        builder.Services.AddScoped<IUserLogService, UserLogService>();

        builder.Services.AddScoped<ITaskLogger, TaskLogger>();
        builder.Services.AddScoped<IMailSender, MailSender.MailSender>();

        builder.Services.AddScoped<IUserLogging, UserLogging>();

        builder.Services.AddAntiforgery(options => options.HeaderName = "XSRF-TOKEN");

        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddSession();

        builder.Logging.ClearProviders();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            //app.UseExceptionHandler("/Error/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }
        app.UseDeveloperExceptionPage();
        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();
        app.UseSession();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Task}/{action=Tasks}");

        app.UseStatusCodePagesWithRedirects("/Error/{0}");

        app.Run();
    }
}
