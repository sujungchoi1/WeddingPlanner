using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace WeddingPlanner.Models
{
    public class Wedding
    {
        [Key]
        public int WeddingId {get;set;}
        
        [Required(ErrorMessage="Please enter a name")]
        [Display(Name="Wedder One")]
        public string WedderOne {get;set;}

        [Required(ErrorMessage="Please enter a name")]
        [Display(Name="Wedder Two")]
        public string WedderTwo {get;set;}

        [Required(ErrorMessage="Please enter a date")]
        [DataType(DataType.Date)]
        public DateTime Date {get;set;} // DateTime? - making it a nullable field

        [Required(ErrorMessage="Please enter an address")]
        [Display(Name="Wedding Address")]
        public string Address {get;set;}
        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;
        

        public List<RSVP> RSVPs {get;set;}


        public int UserId {get;set;}
        public User CreatedBy {get;set;}

    }
}