using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MindClinic.Models
{
    public class Reviews
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string WriterUserId { get; set; }
        public User WriterUser { get; set; }
        [Required]
        public string DoctorUserId { get; set; }
        public User DoctorUser { get; set; }
        [Required]
        public int Rating { get; set; }
        [Required]
        public string Text { get; set; }

        [Required] 
        public DateTime TimeOfReview { get; set; }
       
        [Required] 
        public string Privacy { get; set; }


    }
}
