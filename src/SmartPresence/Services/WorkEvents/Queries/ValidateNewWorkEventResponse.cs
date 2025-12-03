using SmartPresence.Services.WorkEvents.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartPresence.Services.WorkEvents.Queries
{
    public class ValidateNewWorkEventResponse
    {
        public int IdEmployee { get; set; }
        public WorkEventTypeName WorkEventTypeName { get; set; } 
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
