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

        public DbSet<TaskEntity> Task { get; set; }
		public DbSet<TaskStepEntity> TaskStep { get; set; }
		public DbSet<TaskLogEntity> TaskLog { get; set; }
        public DbSet<UserLogEntity> UserLog { get; set; }
        public DbSet<ClientLogEntity> ClientLog { get; set; }
        public DbSet<AddresseeEntity> Addressee { get; set; }
        public DbSet<AddresseeGroupEntity> AddresseeGroup { get; set; }
        public DbSet<TaskStatusEntity> TaskStatuse { get; set; }
        public DbSet<TaskGroupEntity> TaskGroup { get; set; }
		public DbSet<OperationCopyEntity> OperationCopy { get; set; }
		public DbSet<OperationMoveEntity> OperationMove { get; set; }
		public DbSet<OperationReadEntity> OperationRead { get; set; }
		public DbSet<OperationDeleteEntity> OperationDelete { get; set; }
		public DbSet<OperationExistEntity> OperationExist { get; set; }
		public DbSet<OperationRenameEntity> OperationRename { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_configuration.GetConnectionString("DefaultConnection"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TaskOperation>().UseTpcMappingStrategy();
            //modelBuilder.Entity<UserLogEntity>().Property(p=>p.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<UserLogEntity>().HasKey(u => new {u.DateTimeLog, u.UserName});
            
            modelBuilder.Entity<AddresseeEntity>().HasKey(u => new {u.PersonalNumber, u.AddresseeGroupId});
            modelBuilder.Entity<TaskLogEntity>().HasKey(u => new { u.DateTimeLog, u.TaskId});
        }

    }
}
