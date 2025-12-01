using Microsoft.EntityFrameworkCore;
using SmartPresence.Services.Employees.Queries;
using SmartPresence.Services.Shared;
using SmartPresence.Services.Users;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartPresence.Services.Employees
{

    public class EmployeeService : IEmployeeService
    {
        private readonly SmartPresenceDbContext _context;

        private readonly IUserService _userService;
        public EmployeeService(SmartPresenceDbContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        /// Retrieves the organizational information of an employee, including their unique identifier, role, team, and team area within the company.
        public async Task<EmployeeOrganizationInfoResponse> GetEmployeeOrganizationInfo(EmployeeOrganizationInfoByIdRequest request)
        {
            var queryResult = await _context.Employees
                .Where(x => x.Id.Equals(request.Id))
                .Include(y => y.Role)
                .Include(z => z.Team)
                .Include(a => a.Team.Area)
                .Select(b => new EmployeeOrganizationInfoResponse(b))
                .FirstOrDefaultAsync();

            return queryResult;
        }

        // Get all employee work events data which match:
        // Employee id
        // Employee organization level
        // Optionally employee work event status
        // Work events between Begin and End date
        public async Task<List<EmployeeWorkEventsResponse>> GetAllEmployeeWorkEvents(EmployeeWorkEventsRequest request)
        {
            // Get employee id
            var idEmployee = request.IdEmployee;

            // Get employee organization info: role, team and area
            var userOrganizationInfo = await GetEmployeeOrganizationInfo(new EmployeeOrganizationInfoByIdRequest(idEmployee));

            // Create base query
            var baseQuery = _context.Employees.AsQueryable();

            // Add to the query the organizational depth:
            // team level for employees and managers,
            // area level for area managers,
            // full organization for executives,
            // or just the individual on personal profile.
            switch (request.WorkEventEmployeeOrganizationFilter)
            {
                case OrganizationLevelFilter.PERSONAL:
                    baseQuery = baseQuery.Where(x => x.Id.Equals(idEmployee));
                    break;

                case OrganizationLevelFilter.TEAM:
                    baseQuery = baseQuery.Where(x => x.IdTeam.Equals(userOrganizationInfo.Team.Id));
                    break;

                case OrganizationLevelFilter.AREA:
                    baseQuery = baseQuery.Where(x => x.Team.Area.Id.Equals(userOrganizationInfo.Area.Id));
                    break;

                case OrganizationLevelFilter.ALL:
                    break;

                default:
                    return new List<EmployeeWorkEventsResponse>();
            }

            // Add to the query:
            baseQuery = baseQuery
                // Include team info
                .Include(y => y.Team)

                // include work events where
                // 1) the start date and/or the end date are inside the requested date
                // 2) Optionally: filter only given type of work event
                .Include(x => x.WorkEvents
                    .Where(y => (y.StartDate.Date <= request.EndDate.Date && y.EndDate.Date >= request.BeginDate.Date)
                    && (request.WorkEventStatusFilterOut == null || !y.WorkEventStatus.Name.Equals(request.WorkEventStatusFilterOut))))

                // Within work events, include work event status 
                .ThenInclude(z => z.WorkEventStatus)

                // Within work events, include work event type
                .Include(a => a.WorkEvents)
                .ThenInclude(b => b.WorkEventType)

                // Order employee list events putting given employee id on the top, then by team id
                .OrderBy(c => c.Id.Equals(idEmployee) ? 0 : 1)
                    .ThenBy(d => d.IdTeam);

            // Create a list of employee with their work events as response dto classd
            var queryResult = await baseQuery.Select(x => new EmployeeWorkEventsResponse(x)).ToListAsync();

            return queryResult;
        }

        public async Task<EmployeeTimeOffByIdAndYearResponse> GetEmployeeTimeOff(EmployeeTimeOffByIdAndYearRequest request)
        {
            var queryResult = await _context.EmployeeTimeOffs
                .Where(x => x.Id.Equals(request.Id) && x.Year.Equals(request.Year))
                .Select(y => new EmployeeTimeOffByIdAndYearResponse(y))
                .FirstOrDefaultAsync();

            return queryResult;
        }
    }
}
