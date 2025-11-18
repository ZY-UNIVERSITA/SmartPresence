using System.Threading.Tasks;

namespace SmartPresence.Web.SignalR
{
    public interface IPublishDomainEvents
    {
        Task Publish(object evnt);
    }
}
