using Microsoft.EntityFrameworkCore;
using SmartPresence.Services.Users.Queries;
using System;
using System.Linq;
using System.Threading.Tasks;

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
            if (!string.IsNullOrEmpty(user.Email))
            {
                var lowerEmail = user.Email.ToLower();

                return _context.Users
                    .Where(x => x.Email.Equals(lowerEmail))
                    .Select(y => y.Id)
                    .FirstOrDefault();
            }

            return -1;
        }

        public string GetSurnameName(UserIdentificationRequest user)
        {
            if (!string.IsNullOrEmpty(user.Email))
            {
                var userId = GetId(user);

                return _context.Employees
                    .Where(x => x.Id.Equals(userId))
                    .Select(y => string.Concat(y.Surname, " ", y.Name))
                    .FirstOrDefault();
            }

            return string.Empty;
        }
    }


}
