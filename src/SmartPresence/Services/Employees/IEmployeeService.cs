using SmartPresence.Services.Employees.Queries;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartPresence.Services.Employees
{
    public interface IEmployeeService
    {
        public Task<EmployeeOrganizationInfoResponse> GetEmployeeOrganizationInfo(EmployeeOrganizationInfoByIdRequest request);
        public Task<List<EmployeeWorkEventsResponse>> GetAllEmployeeWorkEvents(EmployeeWorkEventsRequest request);
        public Task<EmployeeTimeOffByIdAndYearResponse> GetEmployeeTimeOff(EmployeeTimeOffByIdAndYearRequest request);
    }
}
