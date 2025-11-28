using SmartPresence.Services.Users.Queries;

namespace SmartPresence.Services.Users
{
    public interface IUserService
    {
        public int GetId(UserIdentificationRequest user);
    }
}
