using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MindClinic.Models
{
    public class Schedule
    {
        [Key]
        public int id { get; set; }
        public DateTime startTime { get; set; }
        public DateTime endtime { get; set; }
        public string doctorID { get; set; }
        public User doctor { get; set; }
    }
}
