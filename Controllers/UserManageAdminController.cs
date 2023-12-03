using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;
using WebApplication1.Models;


namespace WebApplication1.Controllers
{
    public class UserManageAdminController : Controller
    {
        UserManager<User> userManager;
        IPasswordHasher<User> passwordHasher;
        ApplicationDbContext _context;
        public UserManageAdminController(UserManager<User> user, IPasswordHasher<User> pass, ApplicationDbContext db)
        {
            userManager = user;
            passwordHasher = pass;
            _context = db;
        }
        public IActionResult Index()
        {
            var AllUsers = userManager.Users;
            return View(AllUsers);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(ProfileViewModel ProfileViewModel)
        {
            if (ModelState.IsValid)
            {
                User appUser = new User
                {
                    FirstName = ProfileViewModel.FirstName,
                    LastName = ProfileViewModel.LastName,
                    Email = ProfileViewModel.Email,
                    UserName = ProfileViewModel.Email,
                    PhoneNumber = ProfileViewModel.PhoneNumber,
                    Description = ProfileViewModel.Description,
                    Languesges = ProfileViewModel.Languesges,
                    Occupation = ProfileViewModel.Occupation,
                    Skills = ProfileViewModel.Skills,
                    Education = ProfileViewModel.Education,
                    Certificate = ProfileViewModel.Certificate,
                    LinkedinAccount = ProfileViewModel.LinkedinAccount,
                    GoogleAccount = ProfileViewModel.GoogleAccount,
                    Gender = ProfileViewModel.Gender,
                    Age = ProfileViewModel.Age,
                    Address = ProfileViewModel.Address
                };

                IdentityResult result = await userManager.CreateAsync(appUser, ProfileViewModel.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View(ProfileViewModel);
        }


        public async Task<IActionResult> Details(string Id)
        {
            var user = await userManager.FindByIdAsync(Id);
            if (user != null)
            {
                return View(user);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string Id)
        {
            if (Id == null)
            {
                return NotFound();
            }

            var user = await userManager.FindByIdAsync(Id);
            if (user == null)
            {
                return NotFound();
            }


            var EditModel = new ProfileViewModel
            {
                ProfileViewModelId = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Password = user.PasswordHash,
                PhoneNumber = user.PhoneNumber,
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
                Address = user.Address


            };
            return View(EditModel);
            //https://localhost:44354/Identity/Account/ConfirmEmail?userId=32ea6fec-1084-4854-a661-fab82e1b601f&code=Q2ZESjhMZ01JY3JlVHNOQ29uOVJmNUpCSTJpT3dKVEhWbUpjQ1JkUzhEOENVSzdkZ3RQMzNycGpPNEczOVhBNlpSNTNJL0dyRWtvMHpRL2RhMU41RVBab3FaRUd1di9lQllGdFJIOHBhVnB1TVBHOWNaL3hHdXVSZ3FQYlJPWW9zRnRlb2Q4WjNrWm1RQnRLQnhRZjRCbmtVdFhrK3h4N3FuMmdlYWdBRmFmK1BCd2J2WHk5K01mM2loekZRMDZkRHBGMjh3M0plSnRSSWpNbjUvMkJGR0ZzdCt2bWdqTFBpUklSQjhNSjNqak5CbnhuNDArVnlYVjhkc1QrYm5tTnJJRmxzdz09

            //string code = await userManager.GenerateEmailConfirmationTokenAsync(user);
            //var confirmationLink = Url.Action("ConfirmEmail", nameof(UsersManageController), new { userId = user.Id, code= code  }, Request.Scheme);
            ////// var message = new Message(new string[] { user.Email }, "Confirmation email link", confirmationLink, null);



        }



        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Edit(ProfileViewModel ProfileViewModel)
        {
            if (ModelState.IsValid)
            {
                var uemail = await userManager.FindByIdAsync(ProfileViewModel.ProfileViewModelId);
                uemail.FirstName = ProfileViewModel.FirstName;
                uemail.LastName = ProfileViewModel.LastName;
                uemail.Email = ProfileViewModel.Email;
                uemail.PasswordHash = passwordHasher.HashPassword(uemail, ProfileViewModel.Password);
                uemail.PhoneNumber = ProfileViewModel.PhoneNumber;
                uemail.Description = ProfileViewModel.Description;
                uemail.Languesges = ProfileViewModel.Languesges;
                uemail.Occupation = ProfileViewModel.Occupation;
                uemail.Skills = ProfileViewModel.Skills;
                uemail.Education = ProfileViewModel.Education;
                uemail.Certificate = ProfileViewModel.Certificate;
                uemail.LinkedinAccount = ProfileViewModel.LinkedinAccount;
                uemail.GoogleAccount = ProfileViewModel.GoogleAccount;
                uemail.Gender = ProfileViewModel.Gender;
                uemail.Age = ProfileViewModel.Age;
                uemail.Address = ProfileViewModel.Address;

                var isSucc = await userManager.UpdateAsync(uemail);
                if (isSucc.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (IdentityError error in isSucc.Errors)
                        ModelState.AddModelError("", error.Description);
                }
            }

            return View(ProfileViewModel);
        }


        [HttpGet]
        public async Task<IActionResult> Delete(string Id)
        {
            var user = await userManager.FindByIdAsync(Id);

            if (user != null)
            {
                var DeletedUser = new ProfileViewModel
                {
                    ProfileViewModelId = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email
                };
                return View(DeletedUser);
            }

            return NotFound();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(ProfileViewModel ProfileViewModel)
        {
            var Deleted = await userManager.FindByIdAsync(ProfileViewModel.ProfileViewModelId);
            if (Deleted != null)
            {
                var result = await userManager.DeleteAsync(Deleted);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
            }
            return View(ProfileViewModel);
        }



    }
}