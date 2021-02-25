using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace WeddingPlanner.Models
{
    public class User
    {
        [Key]
        public int UserId {get;set;}

        [Required(ErrorMessage="Please provide your first name")]
        [MinLength(2, ErrorMessage="Please ensure that your first name has at least 2 characters")]
        [Display(Name="First Name")]
        public string FirstName {get;set;}

        [Required(ErrorMessage="Please provide your last name")]
        [MinLength(2, ErrorMessage="Please ensure that your last name has at least 2 characters")]
        [Display(Name="Last Name")]
        public string LastName {get;set;}

        [Required(ErrorMessage="Please provide your email address")]
        [EmailAddress(ErrorMessage="Please provide a valid email")]
        public string Email {get;set;}

        [Required(ErrorMessage="Please provide your password")]
        [MinLength(8, ErrorMessage="Please ensure that your password has at least 8 characters")]
        [DataType(DataType.Password)]
        public string Password {get;set;}
 
        [NotMapped] // need to add schema
        [Compare("Password", ErrorMessage="Please ensure that the confirmation matches the password")]
        [DataType(DataType.Password)]
        [Display(Name="Confirm Password")]
        public string Confirm {get;set;}
        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;

        public List<Wedding> CreatedWeddings {get;set;}

        public List<RSVP> RSVPs {get;set;}

    }
}