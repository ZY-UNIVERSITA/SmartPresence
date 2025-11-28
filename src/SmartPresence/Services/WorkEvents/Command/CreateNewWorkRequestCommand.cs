using SmartPresence.Services.WorkEvents.Model;
using System;

namespace SmartPresence.Services.WorkEvents.Command
{
    public class CreateNewWorkRequestCommand
    {
        public int IdEmployee { get; set; }
        public WorkEventTypeName WorkEventType { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
