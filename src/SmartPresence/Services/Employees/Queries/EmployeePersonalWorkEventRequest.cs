using SmartPresence.Services.WorkEvents.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartPresence.Services.Employees.Queries
{
    public class EmployeePersonalWorkEventRequest
    {
        public int IdEmployee { get; set; }
        public int Year { get; set; }
        public int Page { get; set; } = 1;
        public int NumberOfResult { get; set; } = 20;
    }
}
