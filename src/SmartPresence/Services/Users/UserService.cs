using SmartPresence.Services.Users.Queries;
using System.Linq;

namespace SmartPresence.Services.Users
{
    public class UserService : IUserService
    {
        private readonly SmartPresenceDbContext _context;

        public UserService(SmartPresenceDbContext context)
        {
            _context = context;
        }

        public int GetId(UserIdentificationRequest user)
        {
            var lowerEmail = user.Email.ToLower();

            return _context.Users
                .Where(x => x.Email.Equals(lowerEmail))
                .Select(y => y.Id)
                .FirstOrDefault();
        }
    }


}
