using Microsoft.AspNetCore.Http;

namespace WebApplication1.Models
{
    public class LoginViewModel
    {
        public string Email { set; get; }
        public string Password { set; get; }
    }


    public class LoginReturnViewModel
    {
        public string Id { get; set; }
        public string Email { set; get; }
        public string FirstName { get; set; }
        public string RoleName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
      
        public string Token { get; set; }

        public string PersonalImage { get; set; }
        public IFormFile ImgProfile { get; set; }


    }

}
