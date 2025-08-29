using FileManager.Extensions;
using FileManager.Infrastructure.Data;
using FileManager.Jobs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Quartz;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddRepositories();
builder.Services.AddServices();

builder.Services.AddAntiforgery(options => options.HeaderName = "XSRF-TOKEN");

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

builder.Services.AddQuartz(options =>
{
    options.SchedulerId = "Scheduler.Core";
    options.SchedulerName = "Quartz.AspNetCore.Scheduler";

    options.MaxBatchSize = 5;
    options.InterruptJobsOnShutdown = true;
    options.InterruptJobsOnShutdownWithWait = true;

    var jobKey = new JobKey("JobTask");
    options.AddJob<JobService>(opts => opts.WithIdentity(jobKey));

    options.AddTrigger(opts => opts
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
});

builder.Services.AddQuartzHostedService(options =>
{
    options.StartDelay = TimeSpan.FromMilliseconds(1_000);
    options.AwaitApplicationStarted = true;
    options.WaitForJobsToComplete = true;
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var schedulerFactory = scope.ServiceProvider.GetService<ISchedulerFactory>();
    var scheduler = await schedulerFactory.GetScheduler();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
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
