using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }

        public string Description { get; set; }

        public string Languesges { get; set; }

        public string Occupation { get; set; }
        public string Skills { get; set; }

        public string Education { get; set; }

        public string Certificate { get; set; }

        public string LinkedinAccount { get; set; }

        public string GoogleAccount { get; set; }


        public string Gender { get; set; }

        public int Age { get; set; }

        public string Address { get; set; }
        public UserImage UserImage { get; internal set; }
    }
}
