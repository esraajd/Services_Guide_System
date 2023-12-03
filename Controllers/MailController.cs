using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using WebApplication1.Models;
using MailKit.Net.Imap;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailController : ControllerBase
    {
        private readonly IMailService mailService;
        public static string c ;
        private UserManager<User> userManager;
        public MailController(IMailService service, UserManager<User> user)
        {
            this.mailService = service;
            userManager = user;
        }

       

        //for sending email
        public string RandomCodeGenerator()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[8];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);
            return finalString;
        }


        [HttpGet("send")]
        [AllowAnonymous]
        public async Task<ActionResult> SendMail(string mail)
        {
            try
            {
                c = RandomCodeGenerator();
                MailRequest mailRequest = new MailRequest
                {
                    Subject = "Services Catalog Forget Code",
                    Body = c,
                    Code = c,
                    ToEmail = mail,
                };

                await mailService.SendEmailAsync(mailRequest);
                Massage m = new Massage
                {
                    massage = c
                    
                };
                return Ok(m);
            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public class Forgetpasword {
            public string code { get; set; }
        }
        [HttpPost("isvalid")]
        public async Task<ActionResult> IsValid(Forgetpasword a)
        {
            if (a.code==c)
            {
                Massage m = new Massage
                {
                    massage = "valid"
                }; 
                return Ok(m);
            }
          
            Massage m1 = new Massage
            {
                    massage = "invalid"
            };
            return Ok(m1);

            
            
          
            
        
        }

        [HttpPost("change")]
        public async Task<ActionResult> ChangePassword(ForgetPasswordViewModel forgetPasswordViewModel)
        {
            var user = await userManager.FindByEmailAsync(forgetPasswordViewModel.Email);

            if (user != null)
            {
                string tk = await userManager.GeneratePasswordResetTokenAsync(user);

                var new_pass = await userManager.ResetPasswordAsync(user, tk, forgetPasswordViewModel.Password);
                if (new_pass.Succeeded)
                {
                    Massage m = new Massage
                    {
                        massage = "The password is reset succefully"
                    };
                    return Ok(m);
                }
            }
            Massage m1 = new Massage
            {
                massage = "Something wrong!"
            };
            return Ok(m1);
        }


    }
}
