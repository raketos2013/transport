using FileManager.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FileManager.DAL
{
    public class AppDbContext : DbContext
    {
        private readonly IConfiguration _configuration;


        public AppDbContext() { }

        public AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            //Database.EnsureDeleted();
            //Database.EnsureCreated();

        }

        public DbSet<TaskEntity> Tasks { get; set; }
        public DbSet<TaskOperationEntity> TaskOperations { get; set; }
        public DbSet<TransportTaskLogEntity> TransportTaskLogs { get; set; }
        public DbSet<UserLogEntity> UserLogs { get; set; }
        public DbSet<ClientLog> ClientLogs { get; set; }
        public DbSet<MailList> MailLists { get; set; }
        public DbSet<MailGroups> MailGroups { get; set; }
        public DbSet<TaskStatusEntity> TaskStatuses { get; set; }
        public DbSet<TaskGroupEntity> TaskGroups { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_configuration.GetConnectionString("DefaultConnection"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<UserLogEntity>().Property(p=>p.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<UserLogEntity>().HasKey(u => new {u.DateTimeLog, u.UserName});
            modelBuilder.Entity<TaskOperationEntity>().HasKey(u => new {u.OperationId, u.TaskId});
            modelBuilder.Entity<MailList>().HasKey(u => new {u.MailGroupsId, u.EMail});
            modelBuilder.Entity<TransportTaskLogEntity>().HasKey(u => new { u.DateTimeLog, u.TaskId, u.OperationId});
        }

    }
}
