using SmartPresence.Services.Employees.Queries;
using SmartPresence.Services.Teams.Queries;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartPresence.Services.Teams
{
    public interface ITeamService
    {
        public Task<TeamInfoResponse> GetTeamsByTeamId(TeamByIdRequest request);
        public Task<List<EmployeeInfoResponse>> GetEmployeeListByTeamId(TeamByIdRequest request);

    }
}
