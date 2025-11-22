namespace SmartPresence.Web.SignalR.Hubs.Events
{
    public class NewMessageEvent
    {
        public int IdGroup { get; set; }

        public int IdUser { get; set; }
        public int IdMessage { get; set; }
    }
}
