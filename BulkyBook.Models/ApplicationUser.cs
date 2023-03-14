using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models
{

    // somehow by adding this model extending the IdentityUser, as well as a new DbSet for ApplicationUser, we were able to add columns to the AspNetUser tables
    // after creating a migration and updating the database
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string Name{ get; set; }
        public string? StreetAddress { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }
    }
}
