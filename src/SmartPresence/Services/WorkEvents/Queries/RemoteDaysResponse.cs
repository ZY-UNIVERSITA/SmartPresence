using SmartPresence.Services.WorkEvents.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartPresence.Services.WorkEvents.Queries
{
    public class RemoteDaysResponse
    {
        public int IdEmployee { get; set; }
        public List<DateTime> Days { get; set; } = new List<DateTime>();
        public List<DateTime> DaysNextWeek { get; set; } = new List<DateTime>();
        public bool Repeat { get; set; } = false;

        public RemoteDaysResponse()
        {

        }

        public RemoteDaysResponse(RemoteDay remoteDay)
        {
            IdEmployee = remoteDay.IdEmployee;
            Days = remoteDay.Days;
            DaysNextWeek = remoteDay.NextWeek;
            Repeat = remoteDay.Repeat;
        }
    }
}
