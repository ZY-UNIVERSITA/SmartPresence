using SmartPresence.Services.WorkEvents.Model;

namespace SmartPresence.Services.WorkEvents.Command
{
    public class HandleWorkEventDecisionCommand
    {
        public int Id { get; set; }
        public WorkEventStatusName Status { get; set; }
    }
}
