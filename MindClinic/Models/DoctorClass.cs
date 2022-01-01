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
        [Display(Name = "Facebook URL")]
        public string FacebookURL { get; set; }
        [Display(Name = "Instagram URL")]
        public string InstagramURL { get; set; }
        [Display(Name = "Youtube URL")]
        public string YoutubeURL { get; set; }
        [Display(Name = "Twitter URL")]
        public string TwitterURL { get; set; }
        [Display(Name = "LinkedIn URL")]
        public string LinkedinURL { get; set; }


    }
}
