using System;
using System.Collections.Generic;
using System.Text;

namespace SmartPresence.Services.Employees.Queries
{
    public class GetEmployeesByNameOrSurnameRequest
    {
        public List<string> Names { get; set; } = new List<String>();

        public GetEmployeesByNameOrSurnameRequest(string names)
        {
            if (!string.IsNullOrWhiteSpace(names))
            {
                foreach (var name in names.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries))
                {
                    Names.Add(name);
                }
            }
        }
    }
}
