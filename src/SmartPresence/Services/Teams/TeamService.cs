using Microsoft.EntityFrameworkCore;
using SmartPresence.Services.Employees.Queries;
using SmartPresence.Services.Teams.Queries;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace SmartPresence.Services.Teams
{
    public class TeamService : ITeamService
    {
        private readonly SmartPresenceDbContext _smartPresenceDbContext;

        public TeamService(SmartPresenceDbContext smartPresenceDbContext)
        {
            _smartPresenceDbContext = smartPresenceDbContext;
        }

        public async Task<TeamInfoResponse> GetTeamsByTeamId(TeamByIdRequest request)
        {
            var queryResult = await _smartPresenceDbContext.Teams
                .Where(x => x.Id.Equals(request.Id))
                .Include(y => y.Employees)
                .Select(z => new TeamInfoResponse(z))
                .FirstOrDefaultAsync();

            return queryResult;
        }

        public async Task<List<EmployeeInfoResponse>> GetEmployeeListByTeamId(TeamByIdRequest request)
        {
            var queryResult = await _smartPresenceDbContext.Teams
                .Where(x => x.Id.Equals(request.Id))
                .Include(y => y.Employees)
                .SelectMany(z => z.Employees)
                .Include(a => a.Role)
                .Include(b => b.Team)
                .Include(c => c.Team.Area)
                .Select(d => new EmployeeInfoResponse(d))
                .ToListAsync();

            return queryResult;
        }
    }
}
