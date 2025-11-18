using SmartPresence.Infrastructure;
using Microsoft.EntityFrameworkCore;
using SmartPresence.Services.Shared;

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
    }
}
