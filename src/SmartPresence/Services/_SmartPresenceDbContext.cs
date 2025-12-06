using Microsoft.EntityFrameworkCore;
using SmartPresence.Infrastructure;
using SmartPresence.Services.Employees;
using SmartPresence.Services.Employees.Model;
using SmartPresence.Services.Shared;
using SmartPresence.Services.WorkEvents.Model;

namespace SmartPresence.Services
{
    public class SmartPresenceDbContext : DbContext
    {
        public SmartPresenceDbContext()
        {
        }

        public SmartPresenceDbContext(DbContextOptions<SmartPresenceDbContext> options) : base(options)
        {
            DataGenerator.InitializeUsers(this);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EmployeeTimeOff>()
                .HasKey(x => new { x.Id, x.Year });
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Area> Areas { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<ContractType> ContractTypes { get; set; }
        public DbSet<WorkEvent> WorkEvents { get; set; }
        public DbSet<WorkEventType> WorkEventTypes { get; set; }
        public DbSet<WorkEventStatus> WorkEventTypeStatus { get; set; }
        public DbSet<EmployeeTimeOff> EmployeeTimeOffs { get; set; }
        public DbSet<RemoteDay> RemoteDays { get; set; }
    }
}
