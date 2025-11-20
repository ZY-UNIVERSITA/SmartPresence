using Microsoft.EntityFrameworkCore;
using SmartPresence.Services.Employees.Queries;
using SmartPresence.Services.Teams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartPresence.Services.Employees
{
    public class EmployeeService : IEmployeeService
    {
        private readonly SmartPresenceDbContext _smartPresenceDbContext;
        public EmployeeService(SmartPresenceDbContext smartPresenceDbContext)
        {
            _smartPresenceDbContext = smartPresenceDbContext;
        }

        /// <summary>
        /// Get function to return all employees
        /// </summary>
        /// <returns>List of all employees</returns>
        public List<GetEmployeesInfoResponse> GetAllEmployees()
        {
            return _smartPresenceDbContext.Employees
                .Select(x => new GetEmployeesInfoResponse(x))
                .ToList();
        }

        /// <summary>
        /// Search function to search in employees list using their id
        /// </summary>
        /// <param name="request">Id of the employee </param>
        /// <returns>List of employees which matches the filter</returns>
        public List<GetEmployeesInfoResponse> GetEmployeesById(GetEmployessByIdRequest request)
        {
            return _smartPresenceDbContext.Employees
                .Where(x => x.Id.Equals(request.Id))
                .Select(y => new GetEmployeesInfoResponse(y))
                .ToList();
        }

        /// <summary>
        /// Search function to search in employees list using their complete or partial name
        /// </summary>
        /// <param name="request">Name and/or surname of the employees</param>
        /// <returns>List of employees which matches the filters</returns>
        public List<GetEmployeesInfoResponse> GetEmployeesByNameOrSurname(GetEmployeesByNameOrSurnameRequest request)
        {
            return _smartPresenceDbContext.Employees
                .Where(x => request.Names.Any(y => x.Name.Contains(y) || x.Surname.Contains(y)))
                .Select(z => new GetEmployeesInfoResponse(z))
                .ToList();
        }
    }
}
