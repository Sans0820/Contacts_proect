using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace Contact_Project.Models
{
    public class Contact
    {
        public int Id { get; set; }
        public string Prefix { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        public string LastName { get; set; }
        public string ContactType { get; set; }
        public string Organization { get; set; }
        public string JobTitle { get; set; }
        public string Department { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }
        public string ImagePath { get; set; }

        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be exactly 10 digits.")]
        public string Phone { get; set; }
        public string Address { get; set; }

        public string Website { get; set; }
    }

}
