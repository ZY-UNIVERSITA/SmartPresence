using Microsoft.EntityFrameworkCore;
using SmartPresence.Services.Employees;
using SmartPresence.Services.Employees.Model;
using SmartPresence.Services.Employees.Queries;
using SmartPresence.Services.Shared;
using SmartPresence.Services.WorkEvents.Command;
using SmartPresence.Services.WorkEvents.Model;
using SmartPresence.Services.WorkEvents.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartPresence.Services.WorkEvents
{
    public class WorkEventService : IWorkEventService, IValidationWorkEventService
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
                IdWorkEventType = idWorkEventType,
                Notes = workEvent.Notes
            };

            _context.WorkEvents.Add(newWorkEvent);
            await _context.SaveChangesAsync();
        }

        // Validazione della richiesta
        public async Task<List<string>> ValidateNewWorkEvent(ValidateNewWorkEventResponse workEvent)
        {
            var problemsList = new List<string>();

            await this.ValidateDateToAvoidMultipleEventsInSameDays(workEvent, problemsList);

            await this.ValidateWorkingDate(workEvent, problemsList);

            this.ValidateLunchHours(workEvent, problemsList);

            return problemsList;
        }

        // Validazione per evitare sovrapposizioni di eventi
        private async Task ValidateDateToAvoidMultipleEventsInSameDays(ValidateNewWorkEventResponse workEvent, List<string> problemsList)
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

            if (await baseQuery.AnyAsync())
            {
                problemsList.Add("Cannot add an event where there is another which will be overlapping");

            }
        }

        // Validazione per evitare di inserire giorni o ore non lavorative
        private async Task ValidateWorkingDate(ValidateNewWorkEventResponse workEvent, List<string> problemsList)
        {
            var baseQuery = _context.ContractTypes.AsQueryable();
            var employeeContract = _context.Employees.AsQueryable();

            var employee = await employeeContract.FirstAsync(x => x.Id.Equals(workEvent.IdEmployee));

            var employeeContractResult = await baseQuery.Where(x => x.Id.Equals(employee.IdContractType)).FirstOrDefaultAsync();

            if (employeeContractResult is not null)
            {
                var eventsDays = Enumerable.Range(0, DateTimeHelper.GetDaysBetweenTwoDates(workEvent.BeginDate, workEvent.EndDate))
                    .Select(x =>
                        new KeyValuePair<DayOfWeek, TimeOnly[]>(
                            workEvent.BeginDate.AddDays(x).DayOfWeek,
                            new[]
                            {
                                new TimeOnly(workEvent.BeginDate.Hour, workEvent.BeginDate.Minute),
                                new TimeOnly(workEvent.EndDate.Hour, workEvent.EndDate.Minute)
                            }
                        )
                    )
                    .ToDictionary(x => x.Key, y => y.Value);

                if (eventsDays.Any(x =>
                {
                    var workHours = employeeContractResult.WorkHours
                        .FirstOrDefault(y => y.Day == x.Key);

                    return workHours != null &&
                        (x.Value[0] < workHours.Start ||
                        x.Value[1] > workHours.End);
                }))
                {
                    problemsList.Add("Cannot add an event where the start or end date is outside the working hours");
                }

            }
        }

        // Validazione per evitare di inserire eventi che comprendano solo l'ora di pranzo
        private void ValidateLunchHours(ValidateNewWorkEventResponse workEvent, List<string> problemsList)
        {
            var lunchStart = new TimeOnly(13, 0);
            var lunchEnd = new TimeOnly(14, 0);

            var eventStart = new TimeOnly(workEvent.BeginDate.Hour, workEvent.BeginDate.Minute);
            var eventEnd = new TimeOnly(workEvent.EndDate.Hour, workEvent.EndDate.Minute);

            if (eventStart >= lunchStart && eventEnd <= lunchEnd)
            {
                problemsList.Add("Cannot add an event where the start and end date is during the lunch hours");
            }
        }

        public async Task ValidatePermissionAsLeave(ValidateNewWorkEventResponse workEvent)
        {
            var baseQuery = _context.ContractTypes.AsQueryable();
            var employeeContract = _context.Employees.AsQueryable();

            var employee = await employeeContract.FirstAsync(x => x.Id.Equals(workEvent.IdEmployee));

            var employeeContractResult = await baseQuery.Where(x => x.Id.Equals(employee.IdContractType)).FirstOrDefaultAsync();

            var day = employeeContractResult.WorkHours.Where(x => x.Day.Equals(workEvent.BeginDate.DayOfWeek)).FirstOrDefault();

            if (day is not null)
            {
                if (day.Start.Equals(new TimeOnly(workEvent.BeginDate.Hour, 0)) && day.End.Equals(new TimeOnly(workEvent.EndDate.Hour, 0)))
                {
                    workEvent.WorkEventTypeName = WorkEventTypeName.HOLIDAY;
                }
            }


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

        // Metodo per accettare/rifiutare tutte le richieste
        public async Task HandleAllWorkEventsDecisions(HandleAllWorkEventsDecisionsCommand command)
        {
            // Trova l'id dello status dell'evento accettato/rifiutato
            var workEventStatus = await _context.WorkEventTypeStatus.Where(x => x.Name.Equals(command.Status)).FirstOrDefaultAsync();

            // Crea una classe helper che raggruppa gli eventi per id dell'employee
            var employeeEvents = await _context.WorkEvents
                .Where(x => command.ListId.Contains(x.Id))
                .Include(x => x.WorkEventType)
                .Include(x => x.Employee)
                    .ThenInclude(x => x.EmployeeTimeOffs)
                .GroupBy(x => x.IdEmployee)
                .Select(y => new WorkEventDecisionHelper()
                {
                    EmployeeId = y.Key,
                    ContractType = y.First().Employee.ContractType,
                    WorkEvents = y.ToList(),
                })
                .ToListAsync();

            if (command.Status.Equals(WorkEventStatusName.APPROVED))
            {
                // Ad ogni employee associa inoltre la lista del timeoff
                employeeEvents.ForEach(async x => x.EmployeeTimeOffs = await _context.EmployeeTimeOffs.Where(y => y.IdEmployee.Equals(x.EmployeeId)).ToListAsync());
            }

            // Ora per ogni employee e per ogni suo evento, cambia lo status dell'evento e aggiorna la tabella del timeoff
            foreach (var employee in employeeEvents)
            {
                foreach (var singleEvent in employee.WorkEvents)
                {
                    // Aggiorna lo status
                    singleEvent.IdWorkEventStatus = workEventStatus.Id;


                    if (command.Status.Equals(WorkEventStatusName.APPROVED))
                    {
                        // Trova le date di inizio e fine
                        var startDate = singleEvent.StartDate;
                        var endDate = singleEvent.EndDate;

                        // Trova la tabella del timeoff dell'anno della richiesta
                        var timeOff = employee.EmployeeTimeOffs.Where(x => x.Year.Equals(startDate.Year)).FirstOrDefault();

                        // Se non esiste una per l'anno della richiesta, ne crea una e lo aggiunge al db
                        if (timeOff is null)
                        {
                            timeOff = new EmployeeTimeOff(employee.ContractType, singleEvent.IdEmployee, startDate.Year);
                            await _context.EmployeeTimeOffs.AddAsync(timeOff);
                        }

                        // Riduce le ore/giorni disponibili
                        if (singleEvent.WorkEventType.Name.Equals(WorkEventTypeName.HOLIDAY))
                        {
                            var days = DateTimeHelper.GetDaysBetweenTwoDates(startDate, endDate);
                            timeOff.HolidayUsed += days;
                        }
                        else
                        {
                            var minutes = (endDate - startDate).TotalMinutes;

                            var lunchStart = startDate.Date.AddHours(13);
                            var lunchEnd = startDate.Date.AddHours(14);

                            var overlapStart = startDate > lunchStart ? startDate : lunchStart;
                            var overlapEnd = endDate < lunchEnd ? endDate : lunchEnd;

                            double lunchMinutes = 0;

                            if (overlapEnd > overlapStart)
                            {
                                lunchMinutes = (overlapEnd - overlapStart).TotalMinutes;
                            }

                            timeOff.LeaveUsed += (minutes - lunchMinutes);

                        }
                    }
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task HandleSingleWorkEventDecision(HandleSingleWorkEventDecisionCommand command)
        {
            // Riutilizza la logica esistente ma per un solo ID
            await HandleAllWorkEventsDecisions(new HandleAllWorkEventsDecisionsCommand
            {
                ListId = new List<int> { command.WorkEventId },
                Status = command.Status
            });
        }

        private class WorkEventDecisionHelper
        {
            public int EmployeeId { get; set; }
            public ContractType ContractType { get; set; }
            public List<EmployeeTimeOff> EmployeeTimeOffs { get; set; }
            public List<WorkEvent> WorkEvents { get; set; }
        }

        // Update remote day 
        public async Task HandleRemoteDays(UpdateRemoteDaysCommand command)
        {
            var remoteDays = await _context.RemoteDays.Where(x => x.IdEmployee.Equals(command.IdEmployee)).FirstOrDefaultAsync();

            if (remoteDays is null)
            {
                remoteDays = new RemoteDay()
                {
                    IdEmployee = command.IdEmployee,
                };

                await _context.RemoteDays.AddAsync(remoteDays);
            }

            remoteDays.Days = command.Days;
            remoteDays.NextWeek = command.DaysNextWeek;
            remoteDays.Repeat = command.Repeat;

            await _context.SaveChangesAsync();
        }

        // Get remote day
        public async Task<RemoteDaysResponse> GetEmployeeRemoteDays(RemoteDaysRequest request)
        {
            var queryResult = await _context.RemoteDays
                .Where(x => x.IdEmployee.Equals(request.IdEmployees))
                .Select(y => new RemoteDaysResponse(y))
                .FirstOrDefaultAsync();

            queryResult ??= new RemoteDaysResponse();

            return queryResult;
        }

        public async Task<WorkEventsCountResponse> GetEmployeeWorkEventPendingTotal(WorkEventsCountRequest request)
        {
            var userId = request.userId;

            var userOrganizationInfo = await _employeeService.GetEmployeeOrganizationInfo(new EmployeeOrganizationInfoByIdRequest(userId));

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
                    return new WorkEventsCountResponse() { IsManager = false };
            }

            var employeeIdList = await baseQuery.Select(x => x.Id).ToHashSetAsync();

            var workEventBaseQueryResult = await _context.WorkEvents
                .Where(x => x.WorkEventStatus.Name.Equals(WorkEventStatusName.PENDING) && employeeIdList.Contains(x.Id))
                .CountAsync();

            return new WorkEventsCountResponse()
            {
                IsManager = true,
                Count = workEventBaseQueryResult
            };
        }
    }
}
