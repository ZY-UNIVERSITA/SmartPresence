using SmartPresence.Services.WorkEvents.Queries;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartPresence.Services.WorkEvents
{
    public interface IValidationWorkEventService
    {
        public Task<List<string>> ValidateNewWorkEvent(ValidateNewWorkEventResponse workEvent);
    }
}
