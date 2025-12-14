using SmartPresence.Services.Users.Queries;
using System.Threading.Tasks;

namespace SmartPresence.Services.Users
{
    public interface IUserService
    {
        public int GetId(UserIdentificationRequest user);
        public string GetSurnameName(UserIdentificationRequest user);
    }
}
