using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MindClinic.Models
{
    public class DoctorClass
    {
        [Key]
        public int id { get; set; }
        [Required]
        public string AboutMe { get; set; }
        [Required]
        public int pricePerSession { get; set; }
        public string userID { get; set; }
        public User User { get; set; }
        public string DefaultMeetingLink { get; set; }
        public double AvgRating { get; set; }
        public int RatingsCount { get; set; }
        public string FacebookURL { get; set; }
        public string InstagramURL { get; set; }
        public string YoutubeURL { get; set; }
        public string TwitterURL { get; set; }
        public string LinkedinURL { get; set; }


    }
}
