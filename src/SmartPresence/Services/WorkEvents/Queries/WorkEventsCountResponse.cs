using System;
using System.Collections.Generic;
using System.Text;

namespace SmartPresence.Services.WorkEvents.Queries
{
    public class WorkEventsCountResponse
    {
        public bool IsManager { get; set; }
        public int Count { get; set; }
    }
}
