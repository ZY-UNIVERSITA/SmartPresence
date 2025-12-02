using SmartPresence.Services.Employees;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartPresence.Services.WorkEvents.Model
{
    public class WorkEvent
    {
        [Key]
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Notes { get; set; } = string.Empty;

        public int IdWorkEventStatus { get; set; }

        [ForeignKey(nameof(IdWorkEventStatus))]
        [InverseProperty("WorkEvents")]
        public WorkEventStatus WorkEventStatus { get; set; }

        public int IdWorkEventType { get; set; }

        [ForeignKey(nameof(IdWorkEventType))]
        [InverseProperty("WorkEvents")]
        public WorkEventType WorkEventType { get; set; }

        public int IdEmployee { get; set; }

        [ForeignKey(nameof(IdEmployee))]
        [InverseProperty("WorkEvents")]
        public Employee Employee { get; set; }
    }
}
