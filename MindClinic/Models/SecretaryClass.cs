using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MindClinic.Models
{
    public class SecretaryClass{
    
        [Key]
        public int Id { get; set; }

        public string SecretaryId { get; set; }
        public User Secretary { get; set; }
        public string DoctorId { get; set; }
        public User Doctor { get; set; }
        public double Salary { get; set; }
        public bool Work { get; set; }

    }
}
