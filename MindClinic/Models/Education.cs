using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MindClinic.Models
{
    public class Education
    {
        [Key]
        public int id { get; set; }
        [Required]
        public string Degree { get; set; }
        [Required]
        public string College { get; set; }
        [Display(Name ="Year Of Completion")]
        [Required]
        public string yearOfCompletion { get; set; }
        public int doctorId { get; set; }
        public DoctorClass doctor { get; set; }

    }
}
