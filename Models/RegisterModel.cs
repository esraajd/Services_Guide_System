using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class RegisterModel
    {
      
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(10, MinimumLength = 6)]
        public string Password { get; set; }

        [Required]
        public string PhoneNumber { get; set; }


        public string RoleName { get; set; }

    }
}
