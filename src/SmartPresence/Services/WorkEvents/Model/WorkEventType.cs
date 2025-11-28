using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartPresence.Services.WorkEvents.Model
{
    public class WorkEventType
    {
        [Key]
        public int Id { get; set; }
        public WorkEventTypeName Name { get; set; }

        public IEnumerable<WorkEvent> WorkEvents { get; set; }
    }

    public enum WorkEventTypeName
    {
        LEAVE,
        HOLIDAY,
        REMOTE
    }
}
