using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MindClinic.Models
{
    public class SecretaryRequests
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string SecretaryID  { get; set; }
        public User Secretary { get; set; }

        [Required]
        public string DoctorId { get; set; }
        public User Doctor { get; set; }

        public double Salary { get; set; }
        public string Message { get; set; }
    }
}
