using SmartPresence.Services.Teams;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SmartPresence.Services.Areas
{
    public class Area
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        public IEnumerable<Team> Teams { get; set; }
    }
}
