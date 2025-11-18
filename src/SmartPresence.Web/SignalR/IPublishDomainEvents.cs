using System.Threading.Tasks;

namespace SmartPresencec.Web.SignalR
{
    public interface IPublishDomainEvents
    {
        Task Publish(object evnt);
    }
}
