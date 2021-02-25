using System;
using System.ComponentModel.DataAnnotations;

namespace WeddingPlanner.Models
{
    public class RSVP
    {
        [Key]
        public int RSVPId {get;set;}

        // connecting to the user
        public int UserId {get;set;}
        public User UserWhoRSVPd {get;set;}

        // connecting to the wedding
        public int WeddingId {get;set;} 
        public Wedding RSVPedWedding {get;set;} 
        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;

    }
}