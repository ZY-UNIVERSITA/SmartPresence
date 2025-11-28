using SmartPresence.Services.WorkEvents.Model;
using System;

namespace SmartPresence.Services.WorkEvents.Command
{
    public class CreateNewWorkRequestCommand
    {
        public int Id { get; set; }
        public WorkEventTypeEnum WorkEventType { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
