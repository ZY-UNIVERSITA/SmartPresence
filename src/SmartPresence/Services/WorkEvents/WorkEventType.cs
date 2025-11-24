using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SmartPresence.Services.WorkEvents
{
    public class WorkEventType
    {
        [Key]
        public int Id { get; set; }
        public WorkEventTypeEnum Name { get; set; }
    
        public IEnumerable<WorkEvent> WorkEvents { get; set; }
    }

    public enum WorkEventTypeEnum
    {
        LEAVE,
        HOLIDAY,
        REMOTE
    }
}
