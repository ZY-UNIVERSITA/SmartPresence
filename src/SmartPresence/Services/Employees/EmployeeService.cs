using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SmartPresence.Services.Employees.Model;
using SmartPresence.Services.Employees.Queries;
using SmartPresence.Services.Shared;
using SmartPresence.Services.WorkEvents.Model;
using SmartPresence.Services.WorkEvents.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartPresence.Services.Employees
{

    public class EmployeeService : IEmployeeService
    {
        private readonly SmartPresenceDbContext _context;

        public EmployeeService(SmartPresenceDbContext context)
        {
            _context = context;
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

        public OrganizationLevelFilter GetEmployeeOrganizationLevelFilter(RoleName role)
        {
            return role switch
            {
                RoleName.EMPLOYEE => OrganizationLevelFilter.TEAM,
                RoleName.TEAM_MANAGER => OrganizationLevelFilter.TEAM,
                RoleName.AREA_MANAGER => OrganizationLevelFilter.AREA,
                RoleName.EXECUTIVE_DIRECTOR => OrganizationLevelFilter.ALL,
                _ => OrganizationLevelFilter.PERSONAL,
            };
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

            // Get employee organization depth level
            var organizationLevelFilter = GetEmployeeOrganizationLevelFilter(userOrganizationInfo.Role.Name);

            // Add to the query the organizational depth:
            // team level for employees and managers,
            // area level for area managers,
            // full organization for executives,
            // or just the individual on personal profile.
            switch (organizationLevelFilter)
            {
                //case OrganizationLevelFilter.PERSONAL:
                //    baseQuery = baseQuery.Where(x => x.Id.Equals(idEmployee));
                //    break;

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

                // Include i giorni da remoto
                .Include(c => c.RemoteDay)

                // Order employee list events putting given employee id on the top, then by team id
                .OrderBy(d => d.Id.Equals(idEmployee) ? 0 : 1)
                    .ThenBy(e => e.IdTeam);

            // Create a list of employee with their work events as response dto classd
            var queryResult = await baseQuery.Select(x => new EmployeeWorkEventsResponse(x)).ToListAsync();


            // PROVE
            var today = DateTime.Now.Date;
            var tomorrow = today.AddDays(1);
            var monday = DateTimeHelper.GetMonday(today);
            var sunday = monday.AddDays(6).Date;

            if (request.EndDate.Date >= tomorrow)
            {
                var workEventStatus = await _context.WorkEventTypeStatus.Where(x => x.Name.Equals(WorkEventStatusName.APPROVED)).FirstOrDefaultAsync();
                var workEventType = await _context.WorkEventTypes.Where(x => x.Name.Equals(WorkEventTypeName.REMOTE)).FirstOrDefaultAsync();

                foreach (var employee in queryResult)
                {
                    var remoteDay = employee.RemoteDay;

                    //if (remoteDay is not null)
                    //{

                    //    Console.WriteLine($"count days {remoteDay.Days.Count}");
                    //    Console.WriteLine($"count days next week: {remoteDay.NextWeek.Count}");
                    //}

                    if (remoteDay is not null && (remoteDay.Days.Count > 0 || remoteDay.NextWeek.Count > 0))
                    {
                        var days = remoteDay.Days;
                        var nextWeekDays = remoteDay.NextWeek;

                        if (remoteDay.NextWeek.Count > 0)
                        {
                            var daysList = new List<DateTime>();
                            daysList.AddRange(remoteDay.Days);
                            daysList.AddRange(remoteDay.NextWeek);

                            Predicate<DateTime> condition = x => x.Date >= tomorrow.Date && x.Date >= request.BeginDate.Date;
                            var daysListIndex = daysList.FindIndex(condition);

                            Console.WriteLine($"Day: {remoteDay.NextWeek.First()}");
                            Console.WriteLine($"Tomorrow: {tomorrow}");
                            Console.WriteLine($"Begin date: {request.BeginDate.Date}");
                            Console.WriteLine(daysListIndex);

                            if (daysListIndex != -1)
                            {
                                for (var i = daysListIndex; i < daysList.Count && daysList[i].Date <= request.EndDate.Date; i++)
                                {
                                    var day = daysList[i].Date;
                                    bool isAnHoliday = !DateTimeHelper.IsAnHoliday(day);
                                    var containsAnHolidayWorkEvent = employee.WorkEventsList
                                        .Where(x => x.WorkEventType.Equals(WorkEventTypeName.HOLIDAY) && x.StartDate <= day && x.EndDate >= day)
                                        .FirstOrDefault();

                                    if (isAnHoliday && containsAnHolidayWorkEvent is null)
                                    {
                                        var workEvent = new WorkEvent()
                                        {
                                            Id = -1,
                                            IdEmployee = employee.Id,
                                            StartDate = day.AddHours(9),
                                            EndDate = day.AddHours(18),
                                            WorkEventStatus = workEventStatus,
                                            WorkEventType = workEventType
                                        };

                                        employee.WorkEventsList.Add(new WorkEventResponse(workEvent));
                                    }
                                }
                            }
                        
                            continue;
                        }

                        DateTime lastUsedDay = today;

                        var addDays = new List<int>();
                        foreach (var day in days)
                        {
                            var dayOfWeek = day.DayOfWeek;
                            if (dayOfWeek.Equals(DayOfWeek.Sunday))
                            {
                                addDays.Add(6);
                            }
                            else
                            {
                                addDays.Add(Convert.ToInt32(dayOfWeek) - 1);
                            }
                        }

                        int index;

                        if (request.BeginDate.Date <= sunday)
                        {
                            index = days.FindIndex(x => (x.Date >= tomorrow) && (x.Date >= request.BeginDate.Date));

                            if (index.Equals(-1))
                            {
                                if (remoteDay.Repeat)
                                {
                                    index = 0;
                                    lastUsedDay = sunday.AddDays(1);
                                }
                            }
                            else
                            {
                                lastUsedDay = monday;
                            }

                        }
                        else
                        {
                            if (!remoteDay.Repeat)
                            {
                                index = -1;
                            }
                            else
                            {
                                var beginDateDayOfWeek = request.BeginDate.DayOfWeek.Equals(DayOfWeek.Sunday) ? 6 : Convert.ToInt32(request.BeginDate.DayOfWeek) - 1;

                                Predicate<int> condition = day => day >= beginDateDayOfWeek;
                                index = addDays.FindIndex(condition);

                                if (index.Equals(-1))
                                {
                                    index = 0;
                                    lastUsedDay = DateTimeHelper.GetMonday(request.BeginDate.Date).AddDays(7);
                                }
                                else
                                {
                                    lastUsedDay = DateTimeHelper.GetMonday(request.BeginDate.Date);
                                }
                            }
                        }

                        var repeat = true;

                        while (!index.Equals(-1) && repeat && lastUsedDay <= request.EndDate.Date)
                        {
                            for (var i = index; i < addDays.Count && lastUsedDay.AddDays(addDays[i]).Date <= request.EndDate.Date; i++)
                            {
                                var day = lastUsedDay.AddDays(addDays[i]);
                                bool isAnHoliday = !DateTimeHelper.IsAnHoliday(day);
                                var containsAnHolidayWorkEvent = employee.WorkEventsList
                                    .Where(x => x.WorkEventType.Equals(WorkEventTypeName.HOLIDAY) && x.StartDate <= day && x.EndDate >= day)
                                    .FirstOrDefault();

                                if (isAnHoliday && containsAnHolidayWorkEvent is null)
                                {
                                    var workEvent = new WorkEvent()
                                    {
                                        Id = -1,
                                        IdEmployee = employee.Id,
                                        StartDate = day.AddHours(9),
                                        EndDate = day.AddHours(18),
                                        WorkEventStatus = workEventStatus,
                                        WorkEventType = workEventType
                                    };

                                    employee.WorkEventsList.Add(new WorkEventResponse(workEvent));
                                }
                            }

                            repeat = employee.RemoteDay.Repeat;

                            lastUsedDay = lastUsedDay.AddDays(7);

                            index = 0;
                        }
                    }

                }
            }
            return queryResult;
        }

        // Restituisce le informazioni visualizzabili nel PROFILO UTENTE
        public async Task<EmployeePersonalWorkEventResponse> GetEmployeePersonalWorkEvent(EmployeePersonalWorkEventRequest request)
        {
            // Get employee id
            var idEmployee = request.IdEmployee;

            // Create base query
            var baseQuery = _context.Employees.AsQueryable();

            // Fitro
            // 1) Mostra solo i dati relativi a quel detemrinato dipendente
            // 2) Include il team
            // 3) Include il ruolo
            // 4) Include gli eventi insieme allo status e al tipo di evento, escludendo per gli eventi remote working
            // 5) Include il contratto del dipendente 
            // 6) Include i dati relativi alle ferie e permessi usati, disponibili, totali per l'anno corrente
            baseQuery = baseQuery
                .Where(x => x.Id.Equals(idEmployee))
                .Include(y => y.Team)
                    .ThenInclude(z => z.Area)
                .Include(a => a.Role)
                .Include(b => b.WorkEvents
                    .Where(c => !c.WorkEventType.Name.Equals(WorkEventTypeName.REMOTE)))
                    .ThenInclude(c => c.WorkEventStatus)
                .Include(b => b.WorkEvents
                    .Where(c => !c.WorkEventType.Name.Equals(WorkEventTypeName.REMOTE)))
                    .ThenInclude(c => c.WorkEventType)
                .Include(d => d.ContractType)
                .Include(e => e.EmployeeTimeOffs
                    .Where(f => f.Year.Equals(request.Year)));

            // Ottieni i risultati e ordina la lista in ordine decrescente per data di inizio
            var queryResult = await baseQuery.Select(x => new EmployeePersonalWorkEventResponse(x)).FirstOrDefaultAsync();
            queryResult.WorkEvents = queryResult.WorkEvents.OrderByDescending(x => x.StartDate).ToList();

            return queryResult;
        }

        // Retrieve employee time off information given employee id and year of search
        public async Task<EmployeeTimeOffByIdAndYearResponse> GetEmployeeTimeOff(EmployeeTimeOffByIdAndYearRequest request)
        {
            var queryResult = await _context.EmployeeTimeOffs
                .Where(x => x.IdEmployee.Equals(request.Id) && x.Year.Equals(request.Year))
                .Select(y => new EmployeeTimeOffByIdAndYearResponse(y))
                .FirstOrDefaultAsync();

            return queryResult;
        }
    }
}
