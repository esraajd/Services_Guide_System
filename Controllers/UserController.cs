using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using Newtonsoft.Json.Linq;
using WebApplication1.Data;
using WebApplication1.Migrations;
using WebApplication1.Models;
using WebApplication1.Services;
using Microsoft.AspNetCore.Hosting;
using Org.BouncyCastle.Ocsp;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private ApplicationDbContext db;
        IPasswordHasher<User> passwordHashe;
        private UserManager<User> userManager;
        private readonly IConfiguration _configration;
        private IWebHostEnvironment Environment;
        public string Imgurl = "https://192.168.77.222:45459/images/";
        public UserController(ApplicationDbContext context,
            UserManager<User> usermanager,
            IPasswordHasher<User> passwordHasher,
            IMailService mailService,
            IConfiguration configration,
            Microsoft.AspNetCore.Hosting.IWebHostEnvironment environment)
        {
            db = context;
            userManager = usermanager;
            this.passwordHashe = passwordHasher;
            _configration = configration;
            Environment = environment;

        }


        [HttpPost]
        [Route("login")]

        public async Task<IActionResult> Login([FromBody] LoginViewModel loginuser)
        {

            try
            {
                var user = await userManager.FindByEmailAsync(loginuser.Email);
                if (user == null)
                {
                    var loginErrorEmail = new Massage
                    {
                        massage = "Invalid Email"
                    };
                    return BadRequest(loginErrorEmail);
                }
                var pass = await userManager.CheckPasswordAsync(user, loginuser.Password);

                if (pass == false)
                {
                    var loginErrorPassword = new Massage
                    {
                        massage = "Invalid password"
                    };
                    return BadRequest(loginErrorPassword);


                }

                else if (user != null && pass != false)
                {

                    var token = new JwtService(_configration);

                    var role = await userManager.GetRolesAsync(user);

                    var imgProfile = db.UserImage.FirstOrDefault(p => p.UserId == user.Id)?.FileName;


                    // string photoname=imgProfile.FileName;
                    // var image = Imgurl+photoname;
                    var loginDone = new LoginReturnViewModel
                    {
                        Id = user.Id,
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        PhoneNumber = user.PhoneNumber,
                        RoleName = role.FirstOrDefault(),
                        PersonalImage = Imgurl + imgProfile,
                        Token = token.GenerateSecurityToken(user.Email, user.Id, role.FirstOrDefault())

                    };


                    return Ok(loginDone);




                }
                return BadRequest("");
            }
            catch (Exception ex) { return BadRequest(ex.Message + "login"); }

        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel registeruser)
        {

            if (ModelState.IsValid)
            {
                User user = new User
                {
                    FirstName = registeruser.FirstName,
                    LastName = registeruser.LastName,
                    Email = registeruser.Email,
                    PhoneNumber = registeruser.PhoneNumber,
                    UserName = registeruser.Email,


                };


                IdentityResult result = await userManager.CreateAsync(user, registeruser.Password);

                if (result.Succeeded)
                {
                    var addToRole = await userManager.AddToRoleAsync(user, registeruser.RoleName);// Roles.Requester);
                    if (addToRole.Succeeded)
                    {
                        Massage response = new Massage
                        {
                            massage = "Susseccfull"

                        };
                        UserImage photo = new UserImage
                        {
                            UserId = user.Id,
                            FileName = "profile.jpg",
                            ContentType = "image/jpeg"
                        };
                        db.UserImage.Add(photo);
                        db.SaveChanges();
                        return Ok(response);


                    }
                    else
                    {
                        Massage response = new Massage
                        {
                            massage = "Error not added to role"

                        };
                        return Ok(response);
                    }
                }
                else
                {
                    foreach (IdentityError error in result.Errors)
                    {

                        Massage response = new Massage
                        {
                            massage = "Email " + registeruser.Email + " is already taken"

                        };
                        return BadRequest(response);
                    }
                }
            }
            else
            {
                Massage response = new Massage
                {
                    massage = "your data is not valid"

                };

                return BadRequest(response);
            }

            return BadRequest("");
        }




        [HttpGet]
        [Route("show")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        public async Task<ActionResult<User>> ShowUserProfile()//string userId)
        {
            //new with jwt 
            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                Massage response = new Massage
                {
                    massage = "Invalid Id" + userId

                };
                return NotFound(response);
            }
            var photouser = db.UserImage.FirstOrDefault(pho => pho.UserId == userId);
            if (photouser == null)
            {
                ProfileViewModel userProfile = new ProfileViewModel
                {
                    UserId = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    PersonalImage = Imgurl + "profile.jpg",
                    Description = user.Description,
                    Languesges = user.Languesges,
                    Occupation = user.Occupation,
                    Skills = user.Skills,
                    Education = user.Education,
                    Certificate = user.Certificate,
                    LinkedinAccount = user.LinkedinAccount,
                    GoogleAccount = user.GoogleAccount,
                    Gender = user.Gender,
                    Age = user.Age,
                    Address = user.Address,

                };

                return Ok(userProfile);

            }
            else
            {
                string photoname = photouser.FileName;
                ProfileViewModel userProfile = new ProfileViewModel
                {
                    UserId = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    PersonalImage = Imgurl + photoname,
                    Description = user.Description,
                    Languesges = user.Languesges,
                    Occupation = user.Occupation,
                    Skills = user.Skills,
                    Education = user.Education,
                    Certificate = user.Certificate,
                    LinkedinAccount = user.LinkedinAccount,
                    GoogleAccount = user.GoogleAccount,
                    Gender = user.Gender,
                    Age = user.Age,
                    Address = user.Address,

                };

                return Ok(userProfile);

            }



        }


        [HttpPost]
        [Route("editprofile")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        public async Task<ActionResult<User>> EditProfile(ProfileViewModel profile)
        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                Massage responseerror = new Massage
                {
                    massage = "Invalid User"
                };
                return NotFound(responseerror);
            }

            // Only update the properties that have been provided
            if (!string.IsNullOrEmpty(profile.FirstName))
            {
                user.FirstName = profile.FirstName;
            }
            if (!string.IsNullOrEmpty(profile.LastName))
            {
                user.LastName = profile.LastName;
            }
            if (!string.IsNullOrEmpty(profile.PhoneNumber))
            {
                user.PhoneNumber = profile.PhoneNumber;
            }
            if (!string.IsNullOrEmpty(profile.Gender))
            {
                user.Gender = profile.Gender;
            }
            if (!string.IsNullOrEmpty(profile.Address))
            {
                user.Address = profile.Address;
            }
            if (profile.Age != null)
            {
                user.Age = profile.Age;
            }
            if (!string.IsNullOrEmpty(profile.Certificate))
            {
                user.Certificate = profile.Certificate;
            }
            if (!string.IsNullOrEmpty(profile.Description))
            {
                user.Description = profile.Description;
            }
            if (!string.IsNullOrEmpty(profile.Education))
            {
                user.Education = profile.Education;
            }
            if (!string.IsNullOrEmpty(profile.GoogleAccount))
            {
                user.GoogleAccount = profile.GoogleAccount;
            }
            if (!string.IsNullOrEmpty(profile.Languesges))
            {
                user.Languesges = profile.Languesges;
            }
            if (!string.IsNullOrEmpty(profile.LinkedinAccount))
            {
                user.LinkedinAccount = profile.LinkedinAccount;
            }
            if (!string.IsNullOrEmpty(profile.Occupation))
            {
                user.Occupation = profile.Occupation;
            }
            if (!string.IsNullOrEmpty(profile.Skills))
            {
                user.Skills = profile.Skills;
            }

            var IsDone = await userManager.UpdateAsync(user);
            await db.SaveChangesAsync();

            if (IsDone.Succeeded)
            {
                Massage response = new Massage
                {
                    massage = "Profile Updated"
                };
                return Ok(response);
            }
            else
            {
                return BadRequest(IsDone.Errors);
            }
        }


        //method to upload image 
        [HttpPost]
        [Route("uploadimage")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        public async Task<IActionResult> Uploadimage([FromForm] LoginReturnViewModel loginwithimage)

        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                Massage responseerror = new Massage
                {
                    massage = "Invalid User"

                };
                return NotFound(responseerror);
            }

            if (loginwithimage.ImgProfile == null || loginwithimage.ImgProfile.Length == 0)
            {
                Massage massgeresponse = new Massage
                {
                    massage = "File not selected!"

                };
                return BadRequest(massgeresponse);
            }
            string m = await UploadImage(loginwithimage.ImgProfile, userId);
            Massagephoto response = new Massagephoto
            {
                massage = m,
                photo = Imgurl + db.UserImage.FirstOrDefault(p => p.UserId == userId).FileName
            };
            return Ok(response);


        }

        public async Task<string> UploadImage(IFormFile ImgProfile, string Id)
        {
            var finduer = db.UserImage.FirstOrDefault(u => u.UserId == Id);
            if (finduer == null)
            {
                //for image 
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images",
               ImgProfile.FileName);
                var stream = new FileStream(path, FileMode.Create);
                await ImgProfile.CopyToAsync(stream);

                using (var memoryStream = new MemoryStream())
                {
                    await ImgProfile.CopyToAsync(memoryStream);
                    var photoData = memoryStream.ToArray();
                    // Use Entities Framework Core to store the photo in the database


                    UserImage photoofuser = new UserImage()
                    {
                        FileName = ImgProfile.FileName,
                        ContentType = ImgProfile.ContentType,
                        UserId = Id
                    };
                    await db.UserImage.AddAsync(photoofuser);
                    await db.SaveChangesAsync();


                }
                //end upload
                return "The Image is added succussfully";
            }
            else
            {
                var path = Path.Combine(Environment.WebRootPath, "images", ImgProfile.FileName);
                using (FileStream stream = new FileStream(path, FileMode.Create))
                {
                    await ImgProfile.CopyToAsync(stream);
                    stream.Close();
                    // Find the existing data
                    var userphoto = db.UserImage.Where(uId => uId.UserId == Id).FirstOrDefault();
                    userphoto.FileName = ImgProfile.FileName;
                    userphoto.ContentType = ImgProfile.ContentType;
                    db.UserImage.Update(userphoto);
                    await db.SaveChangesAsync();
                }




                
                return "The Image is updated successfully";

            }
            return "";


        }



        [HttpPost]
        [Route("changepassword")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                Massage response1 = new Massage
                {
                    massage = "Invalid user id"
                };



                return BadRequest(response1);
            }

            var checkOld = await userManager.CheckPasswordAsync(user, model.OldPassword);

            if (!checkOld)
            {
                Massage response2 = new Massage
                {
                    massage = "Invalid Old Password"
                };

                return BadRequest(response2);
            }

            var newPassword = await userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

            if (newPassword.Succeeded)
            {
                Massage response3 = new Massage
                {
                    massage = "password has been changed"
                };
                return Ok(response3);
            }
            return BadRequest();
        }

    }
}
