using SmartPresence.Services;
using SmartPresence.Services.Areas;
using SmartPresence.Services.Employees;
using SmartPresence.Services.Shared;
using SmartPresence.Services.Teams;
using SmartPresence.Services.WorkEvents.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartPresence.Infrastructure
{
    public class DataGenerator
    {
        private static int nextId = 1;
        public static void InitializeUsers(SmartPresenceDbContext context)
        {
            if (context.Users.Any())
            {
                return;   // Data was already seeded
            }

            AddAreas(context);
            AddContractTypes(context);
            AddIdRoles(context);
            AddIdTeams(context);
            AddEmployees(context);
            AddEventsStatus(context);
            AddWorkEvents(context);
            AddWorkEventType(context);

            AddUsers(context);
        }

        private static void AddAreas(SmartPresenceDbContext context)
        {
            context.Areas.AddRange(
                new Area
                {
                    Id = 1,
                    Name = "Management"
                },
                new Area
                {
                    Id = 2,
                    Name = "Research and Development (R&D)"
                },
                new Area
                {
                    Id = 3,
                    Name = "Finance"
                },
                new Area
                {
                    Id = 4,
                    Name = "Customer Care"
                },
                new Area
                {
                    Id = 5,
                    Name = "Marketing"
                }
            );

            context.SaveChanges();
        }

        private static void AddContractTypes(SmartPresenceDbContext context)
        {
            context.ContractTypes.AddRange(
                new ContractType
                {
                    Id = 1,
                    Name = "Full Time",
                    DailyHours = 8,
                    WeeklyDays = 5
                },
                new ContractType
                {
                    Id = 2,
                    Name = "Part-Time Horizontal",
                    DailyHours = 4,
                    WeeklyDays = 5
                },
                new ContractType
                {
                    Id = 3,
                    Name = "Part-Time Vertical",
                    DailyHours = 8,
                    WeeklyDays = 3
                }
            );

            context.SaveChanges();
        }

        private static void AddIdRoles(SmartPresenceDbContext context)
        {
            context.Roles.AddRange(
                new Role
                {
                    Id = 1,
                    Name = RoleName.EMPLOYEE
                },
                new Role
                {
                    Id = 2,
                    Name = RoleName.TEAM_MANAGER
                },
                new Role
                {
                    Id = 3,
                    Name = RoleName.AREA_MANAGER
                },
                new Role
                {
                    Id = 4,
                    Name = RoleName.EXECUTIVE_DIRECTOR
                }
            );

            context.SaveChanges();
        }

        private static void AddIdTeams(SmartPresenceDbContext context)
        {
            context.Teams.AddRange(
                // R&D Area (Id = 2)
                new Team
                {
                    Id = 1,
                    Name = "Automation",
                    IdArea = 2
                },
                new Team
                {
                    Id = 2,
                    Name = "Computer Vision",
                    IdArea = 2
                },
                new Team
                {
                    Id = 3,
                    Name = "Software Delevopment",
                    IdArea = 2
                },
                new Team
                {
                    Id = 11,
                    Name = "Management",
                    IdArea = 2
                },

                // Finance Area (Id = 3)
                new Team
                {
                    Id = 4,
                    Name = "Accounting",
                    IdArea = 3
                },
                new Team
                {
                    Id = 5,
                    Name = "Budget Planning",
                    IdArea = 3
                },
                new Team
                {
                    Id = 12,
                    Name = "Management",
                    IdArea = 3
                },

                // Customer Care Area (Id = 4)
                new Team
                {
                    Id = 6,
                    Name = "Help Desk",
                    IdArea = 4
                },
                new Team
                {
                    Id = 7,
                    Name = "Technical Support",
                    IdArea = 4
                },
                new Team
                {
                    Id = 13,
                    Name = "Management",
                    IdArea = 4
                },

                // Marketing Area (Id = 5)
                new Team
                {
                    Id = 8,
                    Name = "Digital Marketing",
                    IdArea = 5
                },
                new Team
                {
                    Id = 9,
                    Name = "Brand Strategy",
                    IdArea = 5
                },
                new Team
                {
                    Id = 14,
                    Name = "Management",
                    IdArea = 5
                },

                // Management Area (Id = 1)
                new Team
                {
                    Id = 10,
                    Name = "CDA",
                    IdArea = 1
                }
            );

            context.SaveChanges();
        }

        private static void AddEmployees(SmartPresenceDbContext context)
        {
            string[] firstNames = new string[]
            {
                "Luca","Marco","Giulia","Francesca","Matteo",
                "Alessandro","Chiara","Federico","Sara","Davide",
                "Elena","Simone","Marta","Andrea","Valentina",
                "Giorgio","Silvia","Paolo","Roberta","Stefano"
            };

            string[] lastNames = new string[]
            {
                "Rossi","Ferrari","Russo","Bianchi","Romano",
                "Colombo","Ricci","Marino","Greco","Bruno",
                "Gallo","Conti","De Luca","Mancini","Costa",
                "Giordano","Rizzo","Lombardi","Moretti","Barbieri"
            };

            Random rnd = new Random();
            var employees = new List<Employee>();

            //  CDA (IdTeam Id = 10, Area = 1) 
            for (int i = 0; i < 5; i++)
            {
                employees.Add(new Employee
                {
                    Id = nextId++,
                    Name = firstNames[rnd.Next(firstNames.Length)],
                    Surname = lastNames[rnd.Next(lastNames.Length)],
                    BirthDate = new DateTime(1970 + i, 5, 10),
                    HireDate = new DateTime(2000 + i, 3, 15),
                    IdContractType = 1, // Full Time
                    IdRole = 4,         // Executive Director
                    IdTeam = 10         // CDA
                });
            }

            //  Helper function to add a IdTeam 
            void AddIdTeam(int IdTeamId, int managerIdRole, int employeeIdRole, int minEmployees, int maxEmployees)
            {
                // Manager
                employees.Add(new Employee
                {
                    Id = nextId++,
                    Name = firstNames[rnd.Next(firstNames.Length)],
                    Surname = lastNames[rnd.Next(lastNames.Length)],
                    BirthDate = new DateTime(1980, 1, 1),
                    HireDate = new DateTime(2010, 1, 1),
                    IdContractType = 1,
                    IdRole = managerIdRole,
                    IdTeam = IdTeamId
                });

                // Employees
                int numEmployees = rnd.Next(minEmployees, maxEmployees + 1);
                for (int i = 0; i < numEmployees; i++)
                {
                    employees.Add(new Employee
                    {
                        Id = nextId++,
                        Name = firstNames[rnd.Next(firstNames.Length)],
                        Surname = lastNames[rnd.Next(lastNames.Length)],
                        BirthDate = new DateTime(1985 + i, 2, 20),
                        HireDate = new DateTime(2015, 6, 1),
                        IdContractType = 1,
                        IdRole = employeeIdRole,
                        IdTeam = IdTeamId
                    });
                }
            }

            //  R&D IdTeams 
            AddIdTeam(1, 2, 1, 5, 10); // Automation
            AddIdTeam(2, 2, 1, 5, 10); // Computer Vision
            AddIdTeam(3, 2, 1, 5, 10); // Software Development

            // R&D Area Manager (IdTeam 11)
            employees.Add(new Employee
            {
                Id = nextId++,
                Name = firstNames[rnd.Next(firstNames.Length)],
                Surname = lastNames[rnd.Next(lastNames.Length)],
                BirthDate = new DateTime(1975, 4, 12),
                HireDate = new DateTime(2005, 7, 1),
                IdContractType = 1,
                IdRole = 3, // Area Manager
                IdTeam = 11
            });

            //  Finance IdTeams 
            AddIdTeam(4, 2, 1, 5, 10); // Accounting
            AddIdTeam(5, 2, 1, 5, 10); // Budget Planning

            // Finance Area Manager (IdTeam 12)
            employees.Add(new Employee
            {
                Id = nextId++,
                Name = firstNames[rnd.Next(firstNames.Length)],
                Surname = lastNames[rnd.Next(lastNames.Length)],
                BirthDate = new DateTime(1976, 4, 12),
                HireDate = new DateTime(2006, 7, 1),
                IdContractType = 1,
                IdRole = 3,
                IdTeam = 12
            });

            //  Customer Care IdTeams 
            AddIdTeam(6, 2, 1, 5, 10); // Help Desk
            AddIdTeam(7, 2, 1, 5, 10); // Technical Support

            // Customer Care Area Manager (IdTeam 13)
            employees.Add(new Employee
            {
                Id = nextId++,
                Name = firstNames[rnd.Next(firstNames.Length)],
                Surname = lastNames[rnd.Next(lastNames.Length)],
                BirthDate = new DateTime(1977, 4, 12),
                HireDate = new DateTime(2007, 7, 1),
                IdContractType = 1,
                IdRole = 3,
                IdTeam = 13
            });

            //  Marketing IdTeams 
            AddIdTeam(8, 2, 1, 5, 10); // Digital Marketing
            AddIdTeam(9, 2, 1, 5, 10); // Brand Strategy

            // Marketing Area Manager (IdTeam 14)
            employees.Add(new Employee
            {
                Id = nextId++,
                Name = firstNames[rnd.Next(firstNames.Length)],
                Surname = lastNames[rnd.Next(lastNames.Length)],
                BirthDate = new DateTime(1978, 4, 12),
                HireDate = new DateTime(2008, 7, 1),
                IdContractType = 1,
                IdRole = 3,
                IdTeam = 14
            });

            //  Save all 
            context.Employees.AddRange(employees);
            context.SaveChanges();
        }

        private static void AddUsers(SmartPresenceDbContext context)
        {
            List<User> users = new List<User>();

            for (int i = 0; i < nextId; i++)
            {
                context.Users.Add(
                    new User()
                    {
                        Id = i + 1, // Forced to specific Guid for tests
                        Email = $"email{i + 1}@test.it",
                        Password = "M0Cuk9OsrcS/rTLGf5SY6DUPqU2rGc1wwV2IL88GVGo=", // SHA-256 of text "Prova"
                        FirstName = "Nome1",
                        LastName = "Cognome1",
                        NickName = "Nickname1"
                    }
                );
            }

            context.SaveChanges();
        }

        private static void AddEventsStatus(SmartPresenceDbContext context)
        {
            context.WorkEventTypeStatus.AddRange(
                new WorkEventStatus
                {
                    Id = 1,
                    Name = WorkEventStatusName.APPROVED
                },
                new WorkEventStatus
                {
                    Id = 2,
                    Name = WorkEventStatusName.REFUSED
                },
                new WorkEventStatus
                {
                    Id = 3,
                    Name = WorkEventStatusName.PENDING
                }
            );

            context.SaveChanges();
        }

        private static void AddWorkEventType(SmartPresenceDbContext context)
        {
            context.WorkEventTypes.AddRange(
                new WorkEventType
                {
                    Id = 1,
                    Name = WorkEventTypeEnum.HOLIDAY
                },
                new WorkEventType
                {
                    Id = 2,
                    Name = WorkEventTypeEnum.LEAVE
                },
                new WorkEventType
                {
                    Id = 3,
                    Name = WorkEventTypeEnum.REMOTE
                }
            );

            context.SaveChanges();
        }

        private static void AddWorkEvents(SmartPresenceDbContext context)
        {
            context.WorkEvents.AddRange(
                new WorkEvent
                {
                    Id = 1,
                    StartDate = DateTime.Now.AddDays(3),
                    EndDate = DateTime.Now.AddDays(4).AddMinutes(1),
                    IdWorkEventStatus = 1,
                    IdWorkEventType = 1,
                    IdEmployee = 6,
                },
                new WorkEvent
                {
                    Id = 2,
                    StartDate = DateTime.Now.AddDays(1),
                    EndDate = DateTime.Now.AddDays(2).AddMinutes(1),
                    IdWorkEventStatus = 3,
                    IdWorkEventType = 3,
                    IdEmployee = 7,
                },
                new WorkEvent
                {
                    Id = 3,
                    StartDate = DateTime.Now.AddDays(0),
                    EndDate = DateTime.Now.AddDays(2).AddMinutes(1),
                    IdWorkEventStatus = 3,
                    IdWorkEventType = 3,
                    IdEmployee = 10,
                },
                new WorkEvent
                {
                    Id = 4,
                    StartDate = DateTime.Now.AddDays(2),
                    EndDate = DateTime.Now.AddDays(4).AddMinutes(1),
                    IdWorkEventStatus = 2,
                    IdWorkEventType = 2,
                    IdEmployee = 12,
                },
                new WorkEvent
                {
                    Id = 5,
                    StartDate = DateTime.Now.AddDays(-4),
                    EndDate = DateTime.Now.AddDays(4).AddMinutes(1),
                    IdWorkEventStatus = 1,
                    IdWorkEventType = 1,
                    IdEmployee = 15,
                },
                new WorkEvent
                {
                    Id = 6,
                    StartDate = DateTime.Now.AddDays(1),
                    EndDate = DateTime.Now.AddDays(4).AddMinutes(1),
                    IdWorkEventStatus = 1,
                    IdWorkEventType = 2,
                    IdEmployee = 18,
                },
                new WorkEvent
                {
                    Id = 7,
                    StartDate = DateTime.Now.AddDays(1),
                    EndDate = DateTime.Now.AddDays(4).AddMinutes(1),
                    IdWorkEventStatus = 1,
                    IdWorkEventType = 3,
                    IdEmployee = 17,
                },
                new WorkEvent
                {
                    Id = 8,
                    StartDate = DateTime.Now.AddDays(1),
                    EndDate = DateTime.Now.AddDays(1).AddMinutes(1),
                    IdWorkEventStatus = 3,
                    IdWorkEventType = 2,
                    IdEmployee = 17,
                },
                new WorkEvent
                {
                    Id = 9,
                    StartDate = DateTime.Now.AddDays(0),
                    EndDate = DateTime.Now.AddDays(2).AddMinutes(1),
                    IdWorkEventStatus = 1,
                    IdWorkEventType = 2,
                    IdEmployee = 6,
                },
                new WorkEvent
                {
                    Id = 10,
                    StartDate = DateTime.Now.AddDays(0),
                    EndDate = DateTime.Now.AddDays(2).AddMinutes(1),
                    IdWorkEventStatus = 3,
                    IdWorkEventType = 1,
                    IdEmployee = 16,
                }


            );

            context.SaveChanges();
        }
    }
}
