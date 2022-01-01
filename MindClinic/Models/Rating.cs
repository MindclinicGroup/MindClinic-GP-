using System.ComponentModel.DataAnnotations;

namespace MindClinic.Models
{
    public class Rating
    {
        [Key]
        public int id { get; set; }
        public int rating { get; set; }
        public string patientId { get; set; }
        public User patient { get; set; }
        public string doctorId { get; set; }
        public User doctor { get; set; }
        

    }
}
