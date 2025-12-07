using SmartPresence.Services.WorkEvents.Model;
using System.Collections.Generic;

namespace SmartPresence.Services.WorkEvents.Command
{
    public class HandleSingleWorkEventDecisionCommand
    {
        public int WorkEventId { get; set; }
        public WorkEventStatusName Status { get; set; }
    }
}