using SmartPresence.Services.Employees;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SmartPresence.Services.Shared
{
    public class Role
    {
        [Key]
        public int Id { get; set; }
        public RoleName Name { get; set; }

        public IEnumerable<Employee> Employees { get; set; }
    }

    public enum RoleName
    {
        EMPLOYEE,
        TEAM_MANAGER,
        AREA_MANAGER,
        EXECUTIVE_DIRECTOR
    }
}
