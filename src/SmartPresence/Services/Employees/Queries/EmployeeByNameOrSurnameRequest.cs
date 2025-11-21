using SmartPresence.Services.Users.Queries;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartPresence.Services.Employees.Queries
{
    public class EmployeeByNameOrSurnameRequest
    {
        public UserIdentificationRequest UserIdentificationRequest { get; set; }
        public string Name { get; set; }

        public EmployeeByNameOrSurnameRequest(string email, string names)
        {
            UserIdentificationRequest = new UserIdentificationRequest()
            {
                Email = email,
            };
            Name = names.ToLower();
        }
    }
}
