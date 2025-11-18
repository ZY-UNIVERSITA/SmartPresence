namespace SmartPresence.Services.Shared
{
    public partial class SharedService
    {
        SmartPresenceDbContext _dbContext;

        public SharedService(SmartPresenceDbContext dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
