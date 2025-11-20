using SmartPresence.Services.Employees.Queries;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartPresence.Services.Employees
{
    public interface IEmployeeService
    {
        public List<GetEmployeesInfoResponse> GetAllEmployees();
        public List<GetEmployeesInfoResponse> GetEmployeesById(GetEmployessByIdRequest request);
        public List<GetEmployeesInfoResponse> GetEmployeesByNameOrSurname(GetEmployeesByNameOrSurnameRequest request);
    }
}
