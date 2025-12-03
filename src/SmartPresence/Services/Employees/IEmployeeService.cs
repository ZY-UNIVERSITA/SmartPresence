using SmartPresence.Services.Employees.Model;
using SmartPresence.Services.Employees.Queries;
using SmartPresence.Services.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartPresence.Services.Employees
{
    public interface IEmployeeService
    {
        public Task<EmployeeOrganizationInfoResponse> GetEmployeeOrganizationInfo(EmployeeOrganizationInfoByIdRequest request);
        public Task<List<EmployeeWorkEventsResponse>> GetAllEmployeeWorkEvents(EmployeeWorkEventsRequest request);
        public Task<EmployeePersonalWorkEventResponse> GetEmployeePersonalWorkEvent(EmployeePersonalWorkEventRequest request);
        public Task<EmployeeTimeOffByIdAndYearResponse> GetEmployeeTimeOff(EmployeeTimeOffByIdAndYearRequest request);
        public OrganizationLevelFilter GetEmployeeOrganizationLevelFilter(RoleName role);
    }
}
