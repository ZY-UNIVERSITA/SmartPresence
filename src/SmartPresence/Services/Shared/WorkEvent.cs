using SmartPresence.Services.Employees;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SmartPresence.Services.Shared
{
    public class WorkEvent
    {
        [Key]
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

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
