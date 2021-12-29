using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MindClinic.Models
{
    public class Appointment
    {
        [Key]
        public int id { get; set; }
        public DateTime Time { get; set; }
        public string status { get; set; }
        public string Description { get; set; }
        public string doctorId { get; set; }
        public User doctor { get; set; }
        public string patientId { get; set; }
        public User patient { get; set; }

        public double Price { get; set; }
        public string MeetingLink { get; set; }
    }
}
