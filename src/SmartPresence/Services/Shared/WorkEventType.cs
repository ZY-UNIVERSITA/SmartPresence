using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SmartPresence.Services.Shared
{
    public class WorkEventType
    {
        [Key]
        public int Id { get; set; }
        public int Name { get; set; }
    
        public IEnumerable<WorkEvent> WorkEvents { get; set; }
    }
}
