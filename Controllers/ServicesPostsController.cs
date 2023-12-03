using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using WebApplication1.Data;
using WebApplication1.Models;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.AspNetCore.Identity;
using WebApplication1.Migrations;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicesPostsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IServer server;
        private UserManager<User> userManager;
        public ServicesPostsController(ApplicationDbContext context, IServer server, UserManager<User> userManager)
        {
            _context = context;
            this.server = server;
            this.userManager = userManager;
        }
        public string Imgurl = "https://192.168.77.222:45459/images/";


        [HttpGet]
        [Route("geturl")]
        public IActionResult get()
        {
            var addresses = server.Features.Get<IServerAddressesFeature>().Addresses;
            Console.WriteLine($"Addresses from HomeController.Index: {string.Join(", ", addresses)}");
            return Ok($"Addresses from HomeController.Index: {string.Join(", ", addresses)}");
        }


        //father with childern and posts
        /// <summary>
        /// Doneeeee ^_^
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("showchildernwithposts")]
        public async Task<IActionResult> ShowPostsAndChildCategories(int id)
        {
            var posts = _context.ServicesPosts.Where(i => i.CategoryId == id);
            var postwithlikes = posts.Select(p => new PostViewModel
            {
                ServicesPostId = p.ServicesPostId,
                SearchTag = JsonConvert.DeserializeObject<List<string>>(p.SearchTag),
                Title = p.Title,
                Description = p.Description,
                UserId = p.UserId,
                //  FullName =_context.User.FirstOrDefault(u=>u.Id==p.UserId).FirstName + "" + _context.User.FirstOrDefault(u => u.Id == p.UserId).LastName,
                //Image = _context.User.FirstOrDefault(u => u.Id == p.UserId).PersonalImage,
                Like = _context.Rates.Where(l => l.ServicesPostId == p.ServicesPostId && l.Like == 1).Count(),
                DisLike = _context.Rates.Where(l => l.ServicesPostId == p.ServicesPostId && l.Like == 0).Count()

            }).ToList();

            var categories = _context.Categories
                .FirstOrDefault(p => p.ParentId == id);
            if (categories != null)
            {
                int countofchild = _context.Categories
               .Where(p => p.ParentId == categories.CategoryId).Count();
                if (countofchild > 0)
                {
                    var Catrgorieswithchild = await _context.Categories
                  .Where(p => p.ParentId == id).Select(p => new CategoryModelView
                  {
                      ParentId = id,
                      CategoryId = p.CategoryId,
                      Name = p.Name,
                      Description = p.Description,
                      Image = p.Image,
                      Haschild = false

                  }).ToListAsync();
                    PostsChildViewModel returenPosts1 = new PostsChildViewModel
                    {
                        categories = Catrgorieswithchild,
                        Posts = postwithlikes
                    };

                    return Ok(returenPosts1);

                }
                else
                {
                    var Catrgorieswithoutchild = await _context.Categories
                   .Where(p => p.ParentId == id).Select(p => new CategoryModelView
                   {
                       ParentId = id,
                       CategoryId = p.CategoryId,
                       Name = p.Name,
                       Description = p.Description,
                       Image = p.Image,
                       Haschild = false

                   }).ToListAsync();
                    PostsChildViewModel returenPosts2 = new PostsChildViewModel
                    {
                        categories = Catrgorieswithoutchild,
                        Posts = postwithlikes
                    };

                    return Ok(returenPosts2);

                }

                //var post = await _context.ServicesPosts.FindAsync(id);


            }
            return BadRequest();

        }




        //show 3 posts with image in home
        [HttpGet]
        [Route("showpostimage")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        public async Task<IActionResult> ShowThreePostWithImage()
        {
            //string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var posts = await _context.ServicesPosts.OrderByDescending(order => order.ServicesPostId)
                .Take(3)

                .Select(p => new PostViewModel
                {
                    Title = p.Title,
                    Description = p.Description,
                    SearchTag = JsonConvert.DeserializeObject<List<string>>(p.SearchTag),
                    ServicesPostId = p.ServicesPostId,
                    FullName = p.User.FirstName + " " + p.User.LastName,
                    UserId = p.User.Id,
                    Image = _context.Categories.FirstOrDefault(c => c.CategoryId == p.CategoryId).Image,
                    Like = _context.Rates.Where(l => l.ServicesPostId == p.ServicesPostId && l.Like == 1).Count(),
                    DisLike = _context.Rates.Where(l => l.ServicesPostId == p.ServicesPostId && l.Like == 0).Count()

                }).ToListAsync();

            return Ok(posts);
        }



        //show user posts by id
        //DONEEEEE
        [HttpGet]
        [Route("showpostuser")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]


        public async Task<ActionResult<IEnumerable<PostViewModel>>> ServicesPostsCategory()
        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            var userProfile = _context.User.FirstOrDefault(pro => pro.Id == userId);

            if (userProfile == null)
            {
                Massage responsewitherror = new Massage
                {
                    massage = "No profile for this user"
                };

                return NotFound(responsewitherror);
            }
            
            return await _context.ServicesPosts
              .Where(p => p.UserId == userId)
              .Select(p => new PostViewModel
              {
                  Title = p.Title,
                  Description = p.Description,
                  SearchTag = JsonConvert.DeserializeObject<List<string>>(p.SearchTag),
                  UserId = userId,
                  ServicesPostId = p.ServicesPostId,
                  FullName = p.User.FirstName + " " + p.User.LastName,
                  Image = Imgurl + _context.UserImage.FirstOrDefault(u => u.UserId == userId).FileName,
                  Like = _context.Rates.Where(l => l.ServicesPostId == p.ServicesPostId && l.Like == 1).Count(),
                  DisLike = _context.Rates.Where(l => l.ServicesPostId == p.ServicesPostId && l.Like == 0).Count()
              })
              .ToListAsync();


        }
        //show user posts by id 
        [HttpGet]
        [Route("showpostuserbyid")]


        public async Task<ActionResult<IEnumerable<PostViewModel>>> ServicesPostsCategory(string userID)
        {
            // string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            var userProfile = _context.User.FirstOrDefault(pro => pro.Id == userID);

            if (userProfile == null)
            {
                Massage responsewitherror = new Massage
                {
                    massage = "No profile for this user"
                };

                return NotFound(responsewitherror);
            }
           
            return await _context.ServicesPosts
              .Where(p => p.UserId == userID)
              .Select(p => new PostViewModel
              {
                  Title = p.Title,
                  Description = p.Description,
                  SearchTag = JsonConvert.DeserializeObject<List<string>>(p.SearchTag),
                  UserId = userID,
                  ServicesPostId = p.ServicesPostId,
                  FullName = p.User.FirstName + " " + p.User.LastName,
                  Image = Imgurl + _context.UserImage.FirstOrDefault(u => u.UserId == userID).FileName,
                  Like = _context.Rates.Where(l => l.ServicesPostId == p.ServicesPostId && l.Like == 1).Count(),
                  DisLike = _context.Rates.Where(l => l.ServicesPostId == p.ServicesPostId && l.Like == 0).Count()
              })
              .ToListAsync();


        }
        /// <summary>
        /// DONEEEE
        /// </summary>
        /// <param name="cateId"></param>
        /// <returns></returns>

        //get all posts by category id
        //[Route("viewpostsbycategoryid")]
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<PostViewModel>>> ServicesPostsCategory(int cateId)
        //{
        //    var posts = _context.ServicesPosts.FirstOrDefault(p=>p.CategoryId==cateId || p.SearchTagid.Contains(cateId.ToString()));

        //    if (_context.Photos.FirstOrDefault(p=>p.Id== posts.UserId) != null)
        //    {
        //        return await _context.ServicesPosts

        //        .Where(ps => ps.CategoryId == cateId|| p.SearchTagid.Contains(cateId.ToString()))

        //        .Select(p => new PostViewModel
        //        {
        //            Title = p.Title,
        //            Description = p.Description,
        //            SearchTag = JsonConvert.DeserializeObject<List<string>>(p.SearchTag),
        //            ServicesPostId = p.ServicesPostId,
        //            UserId = p.UserId,
        //            FullName = _context.User.FirstOrDefault(u=>u.Id==p.UserId).FirstName + " " + _context.User.FirstOrDefault(u => u.Id == p.UserId).LastName,
        //            Image = Imgurl + _context.Photos.FirstOrDefault(img => img.Id == p.UserId).FileName,
        //            Like = _context.Rates.Where(l => l.ServicesPostId == p.ServicesPostId && l.Like == 1).Count(),
        //            DisLike = _context.Rates.Where(l => l.ServicesPostId == p.ServicesPostId && l.Like == 0).Count()
        //        }).ToListAsync();

        //    }
        //    return await _context.ServicesPosts

        //       .Where(ps => ps.CategoryId == cateId|| ps.SearchTagid.Contains(cateId.ToString()))

        //       .Select(p => new PostViewModel
        //       {
        //           Title = p.Title,
        //           Description = p.Description,
        //           SearchTag = JsonConvert.DeserializeObject<List<string>>(p.SearchTag),
        //           ServicesPostId = p.ServicesPostId,
        //           UserId = p.UserId,
        //           FullName = _context.User.FirstOrDefault(u => u.Id == p.UserId).FirstName + " " + _context.User.FirstOrDefault(u => u.Id == p.UserId).LastName,
        //           Image = Imgurl+"profile.jpg",
        //           Like = _context.Rates.Where(l => l.ServicesPostId == p.ServicesPostId && l.Like == 1).Count(),
        //           DisLike = _context.Rates.Where(l => l.ServicesPostId == p.ServicesPostId && l.Like == 0).Count()
        //       }).ToListAsync();



        //}
        [Route("viewpostsbycategoryid")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PostViewModel>>> ServicesPostsCategory(int cateId)
        {

            return await _context.ServicesPosts

            .Where(ps => ps.CategoryId == cateId || ps.SearchTagid.Contains(cateId.ToString()))

            .Select(p => new PostViewModel
            {
                Title = p.Title,
                Description = p.Description,
                SearchTag = JsonConvert.DeserializeObject<List<string>>(p.SearchTag),
                ServicesPostId = p.ServicesPostId,
                UserId = p.UserId,
                FullName = _context.User.FirstOrDefault(u => u.Id == p.UserId).FirstName + " " + _context.User.FirstOrDefault(u => u.Id == p.UserId).LastName,
                Image = Imgurl + _context.UserImage.FirstOrDefault(img => img.UserId == p.UserId).FileName,
                Like = _context.Rates.Where(l => l.ServicesPostId == p.ServicesPostId && l.Like == 1).Count(),
                DisLike = _context.Rates.Where(l => l.ServicesPostId == p.ServicesPostId && l.Like == 0).Count()
            }).ToListAsync();




        }
        //[HttpGet]
        //[Route("searchbytagId")]

        //public async Task<IActionResult> Search(int categoryId)
        //{
        //    var result = await _context.ServicesPosts
        //                .Where(r => r.SearchTagid.Contains(categoryId.ToString()))
        //                .ToListAsync();

        //    if (!result.Any())
        //    {
        //        return NotFound("No records found with the specified category id.");
        //    }

        //    return Ok(result);
        //}

        //show posts with user info

        [HttpGet]
        [Route("viewpostwithuserinfo")]

        public async Task<ActionResult<IEnumerable<PostViewModel>>> ShowPostsWithUserInfo()
        {

            var posts = await _context.ServicesPosts
                .Select(p => new PostViewModel
                {
                    Title = p.Title,
                    Description = p.Description,
                    SearchTag = JsonConvert.DeserializeObject<List<string>>(p.SearchTag),
                    ServicesPostId = p.ServicesPostId,
                    UserId = p.UserId,
                    FullName = p.User.FirstName + " " + p.User.LastName,
                    Image = Imgurl + _context.UserImage.FirstOrDefault(img => img.UserId == p.UserId).FileName,
                    Like = _context.Rates.Where(l => l.ServicesPostId == p.ServicesPostId && l.Like == 1).Count(),
                    DisLike = _context.Rates.Where(l => l.ServicesPostId == p.ServicesPostId && l.Like == 0).Count()

                }).ToListAsync();

            if (posts.Count == 0)
            {
                Massage responsewitherror = new Massage
                {
                    massage = "this user has no postos yet"
                };

                return NotFound(responsewitherror);
            }
            return Ok(posts);
        }
        [HttpGet]
        [Route("showprofile")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        public async Task<ActionResult<User>> ShowUserProfile(string userID)//string userId)
        {
            //new with jwt 
            //string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            var user = await userManager.FindByIdAsync(userID);

            if (user == null)
            {
                Massage response = new Massage
                {
                    massage = "Invalid Id" + userID

                };
                return NotFound(response);
            }
            string photouser = _context.UserImage.FirstOrDefault(pho => pho.UserId == userID)?.FileName;
            
         
               
            ProfileViewModel userProfile = new ProfileViewModel
                {
                    UserId = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    PersonalImage = Imgurl + photouser,
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



        //    return NoContent();
        //}
        /// <summary>
        /// dONEEE
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        [HttpPost]
        [Route("createpost")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ProviderUser")]

        public async Task<ActionResult<ServicesPost>> CreatePost([FromBody] ServicePostModelView model)
        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (model == null)
            {
                Massage responsewitherror = new Massage
                {
                    massage = "invalid parameters"
                };
                return BadRequest(responsewitherror);
            }
            List<int> searchid;
            List<string> id=new List<string>();
            foreach (string i in model.SearchTag)
            {
                searchid = _context.Categories.Where(c => c.Name == i).Select(p => p.CategoryId).ToList();
                //  model.SearchTagid.Add(searchid.ToString());
                foreach (int j in searchid)
                {
                    id.Add(j.ToString());


                }




            }
           
            ServicesPost post2 = new ServicesPost
            {
                Title = model.Title,
                Description = model.Description,
                UserId = userId,
                CategoryId = model.CategoryId,
                SearchTag = JsonConvert.SerializeObject(model.SearchTag),
                SearchTagid = JsonConvert.SerializeObject(id)



            };


            await _context.ServicesPosts.AddAsync(post2);
            await _context.SaveChangesAsync();





            Massage response = new Massage
            {
                massage = "Succesfull"
            };
            return Ok(response);
        }


        // POST: api/ServicesPosts

        //[HttpPost]
        //[Authorize(Roles = "ProvideUser")]
        //public async Task<ActionResult<ServicesPost>> PostServicesPost(ServicesPost servicesPost)
        //{
        //    _context.ServicesPosts.Add(servicesPost);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetServicesPost", new { id = servicesPost.ServicesPostId }, servicesPost);
        //}

        //// DELETE: api/ServicesPosts/5
        //[HttpDelete("{id}")]
        //public async Task<ActionResult<ServicesPost>> DeleteServicesPost(int id)
        //{
        //    var servicesPost = await _context.ServicesPosts.FindAsync(id);
        //    if (servicesPost == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.ServicesPosts.Remove(servicesPost);
        //    await _context.SaveChangesAsync();

        //    return servicesPost;
        //}

        private bool ServicesPostExists(int id)
        {
            return _context.ServicesPosts.Any(e => e.ServicesPostId == id);
        }
    }
}
