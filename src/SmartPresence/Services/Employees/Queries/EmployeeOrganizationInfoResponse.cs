using SmartPresence.Services.Employees.Model;

namespace SmartPresence.Services.Employees.Queries
{
    public class EmployeeOrganizationInfoResponse
    {
        public int Id { get; set; }
        public Role Role { get; set; }
        public Team Team { get; set; }
        public Area Area { get; set; }

        public EmployeeOrganizationInfoResponse(Employee employee)
        {
            Id = employee.Id;
            Role = employee.Role;
            Team = employee.Team;
            Area = employee.Team.Area;
        }
    }
}
