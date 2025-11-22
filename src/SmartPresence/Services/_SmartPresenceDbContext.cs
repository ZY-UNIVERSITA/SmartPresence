using SmartPresence.Infrastructure;
using Microsoft.EntityFrameworkCore;
using SmartPresence.Services.Shared;
using SmartPresence.Services.Employees;
using SmartPresence.Services.Teams;
using SmartPresence.Services.Users;
using SmartPresence.Services.Areas;

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

        public DbSet<User> Users { get; set; }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Area> Areas { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<ContractType> ContractTypes { get; set; }

        public DbSet<WorkEvent> WorkEvents { get; set; }
        public DbSet<WorkEventType> WorkEventType { get; set; }
    }
}
