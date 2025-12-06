using SmartPresence.Services.WorkEvents.Model;
using System.Collections.Generic;

namespace SmartPresence.Services.WorkEvents.Command
{
    public class HandleAllWorkEventsDecisionsCommand
    {
        public List<int> ListId { get; set; }
        public WorkEventStatusName Status { get; set; }
    }
}
