using SmartPresence.Services.Employees;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
        [Display(Name = "Employee")]
        EMPLOYEE,

        [Display(Name = "Team Manager")]
        TEAM_MANAGER,

        [Display(Name = "Area Manager")]
        AREA_MANAGER,

        [Display(Name = "Executive Director")]
        EXECUTIVE_DIRECTOR
    }
}
