using SmartPresence.Services.Areas.Queries;
using SmartPresence.Services.Employees.Queries;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartPresence.Services.Areas
{
    public interface IAreaService
    {
        public Task<List<EmployeeInfoResponse>> GetEmployeeInfoByAreaId(AreaByIdRequest query);
    }
}
