using Microsoft.EntityFrameworkCore;
using SmartPresence.Services.Areas.Queries;
using SmartPresence.Services.Employees.Queries;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartPresence.Services.Areas
{
    public class AreaService : IAreaService
    {
        private readonly SmartPresenceDbContext _context;

        public AreaService(SmartPresenceDbContext context)
        {
            _context = context;
        }

        public async Task<List<EmployeeInfoResponse>> GetEmployeeInfoByAreaId(AreaByIdRequest query)
        {
            return await _context.Areas
                .Where(x => x.Id.Equals(query.Id))
                .SelectMany(y => y.Teams)
                .SelectMany(z => z.Employees)
                .Include(a => a.Role)
                .Include(b => b.Team)
                .Include(c => c.Team.Area)
                .Select(d => new EmployeeInfoResponse(d))
                .ToListAsync();
        }
    }
}
