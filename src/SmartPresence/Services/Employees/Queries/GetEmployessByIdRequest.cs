using System;
using System.Collections.Generic;
using System.Text;

namespace SmartPresence.Services.Employees.Queries
{
    public class GetEmployessByIdRequest
    {
        public string Id { get; set; }

        public GetEmployessByIdRequest(string id)
        {
            this.Id = id; 
        }
    }
}
