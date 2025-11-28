using SmartPresence.Services.Shared;
using SmartPresence.Services.WorkEvents.Model;
using System;

namespace SmartPresence.Services.Employees.Queries
{
    public class EmployeeWorkEventsRequest
    {
        public int IdEmployee { get; set; }
        public OrganizationLevelFilter WorkEventEmployeeOrganizationFilter { get; set; }
        public WorkEventStatusName? WorkEventStatusFilterOut { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
