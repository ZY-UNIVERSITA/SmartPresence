using Microsoft.EntityFrameworkCore;
using SmartPresence.Services.Employees;
using SmartPresence.Services.Employees.Model;
using SmartPresence.Services.Employees.Queries;
using SmartPresence.Services.Shared;
using SmartPresence.Services.WorkEvents.Command;
using SmartPresence.Services.WorkEvents.Model;
using SmartPresence.Services.WorkEvents.Queries;
using System.Linq;
using System.Threading.Tasks;

namespace SmartPresence.Services.WorkEvents
{
    public class WorkEventService : IWorkEventService
    {
        private readonly SmartPresenceDbContext _context;
        private readonly IEmployeeService _employeeService;

        public WorkEventService(SmartPresenceDbContext context, IEmployeeService employeeService)
        {
            _context = context;
            _employeeService = employeeService;
        }

        private async Task<int> GetWorkEventStatusByName(WorkEventStatusName name)
        {
            return await _context.WorkEventTypeStatus
                .Where(x => x.Name.Equals(name))
                .Select(y => y.Id)
                .FirstOrDefaultAsync();
        }

        private async Task<int> GetWorkEventTypeByName(WorkEventTypeName name)
        {
            return await _context.WorkEventTypes
                .Where(x => x.Name.Equals(name))
                .Select(y => y.Id)
                .FirstOrDefaultAsync();
        }

        // Crea un nuovo evento
        public async Task HandleNewWorkEvent(CreateNewWorkRequestCommand workEvent)
        {
            // Se l'evento è di tipo REMOTE allora approvalo in automatico altrimenti va in approvazione
            var workEventStatusPending = workEvent.WorkEventType is WorkEventTypeName.REMOTE ? WorkEventStatusName.APPROVED : WorkEventStatusName.PENDING;

            var idWorkEventStatusPending = await this.GetWorkEventStatusByName(workEventStatusPending);
            var idWorkEventType = await this.GetWorkEventTypeByName(workEvent.WorkEventType);

            var newWorkEvent = new WorkEvent()
            {
                IdEmployee = workEvent.IdEmployee,
                StartDate = workEvent.BeginDate,
                EndDate = workEvent.EndDate,
                IdWorkEventStatus = idWorkEventStatusPending,
                IdWorkEventType = idWorkEventType
            };

            _context.WorkEvents.Add(newWorkEvent);
            await _context.SaveChangesAsync();
        }

        // Validazione della richiesta
        public async Task<bool> ValidateNewWorkEvent(ValidateNewWorkEventResponse workEvent)
        {
            var baseQuery = _context.WorkEvents.AsQueryable();

            baseQuery = baseQuery.Where(x => x.IdEmployee.Equals(workEvent.IdEmployee) && !x.WorkEventStatus.Name.Equals(WorkEventStatusName.REFUSED));

            // La query deve controllare 
            // La richiesta non deve iniziare prima della fine di un altro evento
            // La richiesta deve finire prima dell'inizio di un altro evento
            // La richiesta non deve iniziare prima di un altro evento e finire dopo l'evento
            // La richiesta non deve iniziare dopo un altro evento e finire prima della fine dell'evento
            //baseQuery = baseQuery.Where(y =>
            //    workEvent.BeginDate <= y.EndDate ||
            //    workEvent.EndDate >= y.StartDate ||
            //    (workEvent.BeginDate <= y.StartDate && workEvent.EndDate >= y.EndDate) ||
            //    (workEvent.BeginDate >= y.StartDate && workEvent.EndDate <= y.EndDate));

            baseQuery = baseQuery.Where(y =>
                workEvent.BeginDate < y.EndDate &&
                workEvent.EndDate > y.StartDate);

            return await baseQuery.AnyAsync();
        }

        // Restituisce le richieste create dai dipendenti che devono ancora essere approvate dal manager
        public async Task<EmployeePendingWorkEventsResponse> GetEmployeeWorkEventsPending(EmployeePendingWorkEventsRequest request)
        {
            // Get employee id
            var idEmployee = request.IdEmployee;

            // Get employee organization info: role, team and area
            var userOrganizationInfo = await _employeeService.GetEmployeeOrganizationInfo(new EmployeeOrganizationInfoByIdRequest(idEmployee));

            // Create base query
            var baseQuery = _context.Employees.AsQueryable();

            // Pending request is available only to Manager level (TEAM, AREA or being an EXECUTIVE)
            var organizationLevelFilter = userOrganizationInfo.Role.Name.Equals(RoleName.EMPLOYEE)
                ? OrganizationLevelFilter.PERSONAL
                : _employeeService.GetEmployeeOrganizationLevelFilter(userOrganizationInfo.Role.Name);

            switch (organizationLevelFilter)
            {
                case OrganizationLevelFilter.TEAM:
                    baseQuery = baseQuery.Where(x => x.IdTeam.Equals(userOrganizationInfo.Team.Id));
                    break;

                case OrganizationLevelFilter.AREA:
                    baseQuery = baseQuery.Where(x => x.Team.Area.Id.Equals(userOrganizationInfo.Area.Id));
                    break;

                case OrganizationLevelFilter.ALL:
                    break;

                default:
                    return new EmployeePendingWorkEventsResponse();
            }

            // Filtro:
            // 1) Restituisce una lista dei dipendenti che hanno degli eventi in PENDING
            // 2) Include nella lista degli eventi di tipo pending di ogni dipendente anche i dati relativi allo status della richiesta
            baseQuery = baseQuery
                .Where(x => x.WorkEvents.Any(y => y.WorkEventStatus.Name.Equals(WorkEventStatusName.PENDING)))
                //.Include(z => z.WorkEvents
                //    .Where(a => a.WorkEventStatus.Name.Equals(WorkEventStatusName.PENDING)))
                //    .ThenInclude(b => b.WorkEventStatus)
                .Include(z => z.WorkEvents
                    .Where(a => a.WorkEventStatus.Name.Equals(WorkEventStatusName.PENDING)))
                    .ThenInclude(b => b.WorkEventType);

            // A partire dai risultati precedenti, crea la lista dei singoli eventi
            // Select many spacchetta la lista degli eventi del singolo dipendente in singoli eventi indipendenti
            // Seleziona solo gli eventi in PENDING e a partire da questi eventi filtrati, crea una classe che rappresenta il singolo evento indipendente
            var queryResult = await baseQuery
                .SelectMany(
                    x => x.WorkEvents.Where(y => y.WorkEventStatus.Name.Equals(WorkEventStatusName.PENDING)),
                    (x, y) => new SingleWorkEvent(x, y))
                .ToListAsync();

            var response = new EmployeePendingWorkEventsResponse()
            {
                ListEvents = queryResult.OrderBy(z => z.BeginDate).ToList()
            };

            return response;
        }

        // Metodo per accettare/rifiutare le richieste
        public async Task HandleWorkEventDecision(HandleWorkEventDecisionCommand command)
        {
            var workEvent = await _context.WorkEvents.Where(x => x.Id.Equals(command.Id)).FirstOrDefaultAsync();
            var workEventStatus = await _context.WorkEventTypeStatus.Where(x => x.Name.Equals(command.Status)).FirstOrDefaultAsync();

            if (workEvent is not null && workEventStatus is not null)
            {
                workEvent.IdWorkEventStatus = workEventStatus.Id;

                await _context.SaveChangesAsync();
            }
        }

        // Metodo per accettare/rifiutare tutte le richieste
        public async Task HandleAllWorkEventsDecisions(HandleAllWorkEventsDecisionsCommand command)
        {
            var workEventStatus = await _context.WorkEventTypeStatus.Where(x => x.Name.Equals(command.Status)).FirstOrDefaultAsync();

            var workEvents = await _context.WorkEvents.Where(x => command.ListId.Contains(x.Id)).ToListAsync();

            foreach (var singleEvent in workEvents)
            {
                singleEvent.IdWorkEventStatus = workEventStatus.Id;
            }

            await _context.SaveChangesAsync();
        }
    }
}
