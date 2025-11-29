using System;
using System.Collections.Generic;
using System.Text;

namespace SmartPresence.Services.Employees.Queries
{
    public class EmployeeTimeOffByIdAndYearRequest
    {
        public int Id { get; set; }
        public int Year { get; set; }
    }
}
