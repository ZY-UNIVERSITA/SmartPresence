using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartPresence.Services.WorkEvents.Model
{
    public class WorkEventStatus
    {
        [Key]
        public int Id { get; set; }
        public WorkEventStatusName Name { get; set; }

        public IEnumerable<WorkEvent> WorkEvents { get; set; }
    }

    public enum WorkEventStatusName
    {
        [Display(Name = "Approved")]
        APPROVED,

        [Display(Name = "Pending")]
        PENDING,

        [Display(Name = "Refused")]
        REFUSED
    }
}
