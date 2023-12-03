using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class ProfileViewModel
    {
        public string ProfileViewModelId { get; set; }
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }

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
        public string PersonalImage { get; set; }
        


    }



}
