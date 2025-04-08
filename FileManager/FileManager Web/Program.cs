using FileManager.DAL;
using FileManager.DAL.Repositories.Interfaces;
using FileManager.DAL.Repositories.Implementations;
using FileManager.Services.Implementations;
using FileManager.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using FileManager_Web.Logging;




namespace FileManager_Web
{
	public class Program
    {
        public static void Main(string[] args)
        {
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
			builder.Services.AddScoped<ITaskRepository, TaskRepository>();
            builder.Services.AddScoped<IStepRepository, StepRepository>();
            builder.Services.AddScoped<IAddresseeRepository, AddresseeRepository>();
            builder.Services.AddScoped<IOperationRepository, OperationRepository>();
            builder.Services.AddScoped<ITaskService, TaskService>();
			builder.Services.AddScoped<IStepService, StepService>();
            builder.Services.AddScoped<IAddresseeService, AddresseeService>();
            builder.Services.AddScoped<IOperationService, OperationService>();
            builder.Services.AddHttpContextAccessor().AddHttpContextAccessor();
            
            builder.Logging.ClearProviders();
           /* builder.Logging.AddDbLogger(options =>
            {
                {
                    builder.Configuration.GetSection("Database").GetSection("Options").Bind(options);
                }
            });*/
            builder.Services.AddTransient<UserLogging>();
            

            var app = builder.Build();
            
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                //app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
       
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Task}/{action=Tasks}");

            app.Run();

        }
    }
}
