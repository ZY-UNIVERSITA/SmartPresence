using SmartPresence.Services.Employees;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace SmartPresence.Services.WorkEvents.Model
{
    public class RemoteDay
    {
        [Key]
        public int Id { get; set; }
        public int IdEmployee { get; set; }

        [ForeignKey(nameof(IdEmployee))]
        [InverseProperty("RemoteDay")]
        public Employee Employee { get; set; }

        public List<DateTime> Days { get; set; }
        public bool Repeat { get; set; }
    }
}
