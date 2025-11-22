using SmartPresence.Services.Users.Queries;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartPresence.Services.Users
{
    public interface IUserService
    {
        public int GetId(UserIdentificationRequest user);
    }
}
