using Microsoft.Extensions.DependencyInjection;
using SmartPresence.Services.Areas;
using SmartPresence.Services.Employees;
using SmartPresence.Services.Shared;
using SmartPresence.Services.Teams;
using SmartPresence.Services.Users;
using SmartPresence.Web.SignalR;

namespace SmartPresence.Web
{
    public class Container
    {
        public static void RegisterTypes(IServiceCollection container)
        {
            // Registration of all the database services you have
            container.AddScoped<SharedService>();

            // Registration of SignalR events
            container.AddScoped<IPublishDomainEvents, SignalrPublishDomainEvents>();

            // Register custom service
            container.AddScoped<IEmployeeService, EmployeeService>();
            container.AddScoped<IUserService, UserService>();
            container.AddScoped<ITeamService, TeamService>();
            container.AddScoped<IAreaService, AreaService>();

        }
    }
}
