using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApplication1.Data;
using WebApplication1.Migrations;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingApiController : ControllerBase
    {
        private ApplicationDbContext _context;
        public RatingApiController(ApplicationDbContext db)
        {
            _context = db;
        }



        [HttpPost]
        [Route("Ratelike")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        public async Task<IActionResult> Ratelike([FromBody] Rate ratefrombody)
        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            //int totalRate = 0;
            if (userId == null)
            {


                Massage response = new Massage
                {
                    massage = "User Not Found"

                };

                return BadRequest(response);
            }
            
            try
            {
                var user= _context.Rates.FirstOrDefault(p =>( p.UserId == userId) &&( p.ServicesPostId == ratefrombody.ServicesPostId))?.UserId;

                if (user != null && ratefrombody.Like == 1)
                {
                    ////every think is okkkkk

                    var userlikes = _context.Rates.FirstOrDefault(p => (p.UserId == user) && (p.ServicesPostId == ratefrombody.ServicesPostId))?.Like;

                    if (userlikes== 0 && ratefrombody.Like == 1) { 
                        var rate2 = new Rate
                        {
                            UserId = userId,
                            ServicesPostId = ratefrombody.ServicesPostId,
                            Like = ratefrombody.Like
                        };
                        Rate updateuserlike = _context.Rates.FirstOrDefault(p => (p.UserId == rate2.UserId) && (p.ServicesPostId == rate2.ServicesPostId) && (p.Like == 0));
                        updateuserlike.UserId = userId;
                        updateuserlike.ServicesPostId = rate2.ServicesPostId;
                        updateuserlike.Like = 1;

                        var Doneupdate = _context.Rates.Update(updateuserlike);
                        await _context.SaveChangesAsync();
                        var totalRatelike = _context.Rates
                        .Where(x => x.ServicesPostId == rate2.ServicesPostId && x.Like==1)
                        .Sum(x => x.Like);
                        var totalRatedislike = _context.Rates
                        .Where(x => x.ServicesPostId == rate2.ServicesPostId && x.Like == 0)
                        .Count();
                        likes like = new likes
                        {
                            Like = totalRatelike,
                            Dislike = totalRatedislike
                        };
                        return Ok(like);

                    }
                    else if ( ratefrombody.Like == 1 && userlikes == 1)
                        {
                            ////every think is okkkkkkk
                            Massage responseike = new Massage
                            {
                                massage = "You already like"

                            };

                            return Ok(responseike);
                        }

                   
                     


                }
                else if (user == null && ratefrombody.Like == 1)
                {
                    Rate ratefromuser = new Rate
                    {
                        UserId = userId,
                        ServicesPostId = ratefrombody.ServicesPostId,
                        Like = ratefrombody.Like
                    };

                    _context.Rates.Add(ratefromuser);
                    _context.SaveChanges();
                    var totalRatelike = _context.Rates
                   .Where(x => x.ServicesPostId == ratefrombody.ServicesPostId && x.Like == 1)
                   .Sum(x => x.Like);
                    var totalRatedislike = _context.Rates
                   .Where(x => x.ServicesPostId == ratefrombody.ServicesPostId && x.Like == 0)
                   .Count();


                    likes like = new likes
                    {
                        Like = totalRatelike,
                        Dislike = totalRatedislike
                    };
                    return Ok(like);


                }

            }catch(NullReferenceException nre)
            {
               return BadRequest( nre);
            }
           



            return Ok("");

        }
        [HttpPost]
        [Route("Ratedislike")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        public async Task<IActionResult> Ratedislike([FromBody] Rate ratefrombody)
        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {


                Massage response = new Massage
                { 
                    massage = "User Not Found"

                };

                return BadRequest(response);
            }

            try
            {
                var user = _context.Rates.FirstOrDefault(p => (p.UserId == userId) && (p.ServicesPostId == ratefrombody.ServicesPostId))?.UserId;

                /// return Ok(user);
                if (user != null && ratefrombody.Like == 0)
                {
                    ////every think is okkkkk

                    var userlikes = _context.Rates.FirstOrDefault(p => (p.UserId == user) && (p.ServicesPostId == ratefrombody.ServicesPostId))?.Like;

                    if (ratefrombody.Like == 0 && userlikes==1 )
                    {
                        var rate2 = new Rate
                        {
                            UserId = userId,
                            ServicesPostId = ratefrombody.ServicesPostId,
                            Like = ratefrombody.Like
                        };
                        Rate updateuserlike = _context.Rates.FirstOrDefault(p => (p.UserId == rate2.UserId) && (p.ServicesPostId == rate2.ServicesPostId) && (p.Like == 1));
                        updateuserlike.UserId = userId;
                        updateuserlike.ServicesPostId = rate2.ServicesPostId;
                        updateuserlike.Like = 0;

                        var Doneupdate = _context.Rates.Update(updateuserlike);
                        await _context.SaveChangesAsync();
                        var totalRatedislike = _context.Rates
                        .Where(x => x.ServicesPostId == rate2.ServicesPostId && x.Like == 0)
                        .Count();
                        var totalRatelike = _context.Rates
                        .Where(x => x.ServicesPostId == rate2.ServicesPostId && x.Like == 1)
                        .Sum(x => x.Like);
                        likes like = new likes
                        {
                            Like = totalRatelike,
                            Dislike = totalRatedislike
                        };
                        return Ok(like);

                    }
                    else if (ratefrombody.Like == 0 && userlikes==0)
                    {
                        ////every think is okkkkkkk
                        Massage responseike = new Massage
                        {
                            massage = "You already Dislike"

                        };

                        return Ok(responseike);
                    }





                }
                else if (user == null && ratefrombody.Like == 0)
                {
                    Rate ratefromuser = new Rate
                    {
                        UserId = userId,
                        ServicesPostId = ratefrombody.ServicesPostId,
                        Like = ratefrombody.Like
                    };

                    _context.Rates.Add(ratefromuser);
                    _context.SaveChanges();
                    var totalRatedislike = _context.Rates
                   .Where(x => x.ServicesPostId == ratefrombody.ServicesPostId && x.Like==0)
                   .Count();
                    var totalRatelike = _context.Rates
                       .Where(x => x.ServicesPostId == ratefrombody.ServicesPostId && x.Like == 1)
                       .Sum(x => x.Like);
                    likes like = new likes
                    {
                        Like = totalRatelike,
                        Dislike = totalRatedislike
                    };
                    return Ok(like);


                }

            }
            catch (NullReferenceException nre)
            {
                return BadRequest(nre);
            }



            return Ok("");

        }
        [HttpGet]
        [Route("showlikes")]
        public async Task<IActionResult> showlikes(int postid)
        {
            var Dislikes = _context.Rates
                  .Where(x => x.ServicesPostId == postid && x.Like == 0)
                  .Count();
            var likes = _context.Rates
                 .Where(x => x.ServicesPostId == postid && x.Like == 1)
                       .Sum(x => x.Like);
            likes like = new likes
            {
                Like = likes,
                Dislike = Dislikes
            };

            return Ok(like);
        }


    }
}


//[HttpGet]
//[Route("totalrate")]
//[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

//public async Task<ActionResult> GetTotalRate(int postId)
//{
//    double totalRate = 0;
//    var post = await _context.ServicesPosts.FirstOrDefaultAsync(p => p.ServicesPostId == postId);

//    var rateNum = post.Rate.StarsPoints;
//    totalRate += rateNum;
//    return Ok(totalRate);
//}




