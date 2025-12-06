using System;
using System.Collections.Generic;
using System.Text;

namespace SmartPresence.Services.WorkEvents.Command
{
    public class UpdateRemoteDaysCommand
    {
        public int IdEmployee { get; set; }
        public List<DateTime> Days { get; set; }
        public bool Repeat { get; set; }
    }
}
