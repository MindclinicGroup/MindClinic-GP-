using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MindClinic.Models
{
    public class Awards
    {
        [Key]
        public int id { get; set; }
        [Required]
        [Display(Name ="Award")]
        public string award { get; set; }
        [Required]
        public string Year { get; set; }
        public int doctorId { get; set; }
        public DoctorClass doctor { get; set; }
        //


    }
}
