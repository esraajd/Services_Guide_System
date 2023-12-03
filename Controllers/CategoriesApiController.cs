using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;
using WebApplication1.Data;
using WebApplication1.Migrations;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesApiController : ControllerBase
    {
        private ApplicationDbContext _context;

        public CategoriesApiController(ApplicationDbContext context)
        {
            _context = context;
        }
        public string Imgurl = "https://192.168.77.222:45459/images/";


        [HttpGet]
        [Route("showtree")]
        public async Task<IActionResult> showtree()
        {
            var allCategories = await _context.Categories.ToListAsync();
            var rootCategories = allCategories.Where(c => c.ParentId == null).ToList();
            var jsonObject = new ClosedJsonObject();
            jsonObject.categories = new List<ClosedJsonCategory>();

            // Initialize depth counter
            int depth = 0;

            foreach (var category in rootCategories)
            {
                jsonObject.categories.Add(BuildClosedCategoryObject(category, allCategories, depth));
            }
            return Ok(jsonObject);
        }

        private ClosedJsonCategory BuildClosedCategoryObject(Category category, List<Category> allCategories, int depth)
        {
            // Increment depth counter
            depth++;

            var children = allCategories.Where(c => c.ParentId == category.CategoryId).ToList();
            var childCategories = new List<ClosedJsonCategory>();
            foreach (var child in children)
            {
                // Check if the current depth is less than or equal to 5
                if (depth <= 13)
                {
                    childCategories.Add(BuildClosedCategoryObject(child, allCategories, depth));
                }
            }
            return new ClosedJsonCategory
            {
                id = category.CategoryId,
                name = category.Name,
                children = childCategories
            };
        }
       

            [HttpGet]
        [Route("showparent")]
        public async Task<IActionResult> ShowParentCategories()
        {

            var categories = _context.Categories
               .FirstOrDefault(p => p.ParentId == null);
            if (categories != null)
            {
                int countofchild = _context.Categories
               .Where(p => p.ParentId == categories.CategoryId).Count();
                if (countofchild > 0)
                {
                    var Catrgorieswithchild = await _context.Categories
                    .Where(p => p.ParentId == null).Select(p => new CategoryModelView
                    {
                        ParentId = null,
                        CategoryId = p.CategoryId,
                        Name = p.Name,
                        Description = p.Description,
                        Image = p.Image,
                        Haschild = true

                    }).ToListAsync();
                    return Ok(Catrgorieswithchild);
                }
            }
                    var json = System.Text.Json.JsonSerializer.Serialize(categories, new JsonSerializerOptions()
            {
                WriteIndented = true,
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            });

            return Ok(json);
        }
        
       
        public class ClosedJsonObject
        {
            public List<ClosedJsonCategory> categories { get; set; }
        }
        public class ClosedJsonCategory
        {
            public int id { get; set; }
            public string name { get; set; }
            public List<ClosedJsonCategory> children { get; set; }
        }
        [HttpGet]
        [Route("showallcategories")]
        public async Task<IActionResult> ShowAllCategories()
        {
            var allCategories = await _context.Categories.ToListAsync();
            var rootCategories = allCategories.Where(c => c.ParentId == null).ToList();
            var jsonObject = new ClosedJsonObject();
            jsonObject.categories = new List<ClosedJsonCategory>();
            foreach (var category in rootCategories)
            {
                jsonObject.categories.Add(BuildClosedCategoryObject(category, allCategories));
            }
            return Ok(jsonObject);
        }

        private ClosedJsonCategory BuildClosedCategoryObject(Category category, List<Category> allCategories)
        {
            var children = allCategories.Where(c => c.ParentId == category.CategoryId).ToList();
            var childCategories = new List<ClosedJsonCategory>();
            foreach (var child in children)
            {
                childCategories.Add(BuildClosedCategoryObject(child, allCategories));
            }
            return new ClosedJsonCategory
            {
                id = category.CategoryId,
                name = category.Name,
                children = childCategories
            };
        }
        //for childern
        [HttpGet]
        [Route("showchildern")]
        public async Task<IActionResult> ShowChildCategories(int id)
        {

            var categories =  _context.Categories
                .FirstOrDefault(p => p.ParentId == id);
            if (categories!=null)
            {
                int countofchild =_context.Categories
               .Where(p => p.ParentId == categories.CategoryId).Count();
                if (countofchild > 0)
                {
                   var Catrgorieswithchild = await _context.Categories
                   .Where(p => p.ParentId ==id).Select(p => new CategoryModelView
                   {
                       ParentId = id,
                       CategoryId = p.CategoryId,
                       Name = p.Name,
                       Description = p.Description,
                       Image = p.Image,
                       Haschild = true

                   }).ToListAsync();
                    return Ok(Catrgorieswithchild);

                }
                else
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
                    return Ok(Catrgorieswithchild);

                }
            
           

            }
            return Ok("");

            
        }

      


        

        

        private void GetAllChildren(Category parentCategory, List<Category> allCategories, List<Category> result)
        {
            result.Add(parentCategory);
            parentCategory.Children = allCategories.Where(c => c.ParentId == parentCategory.CategoryId).ToList();
            foreach (var child in parentCategory.Children)
            {
                GetAllChildren(child, allCategories, result);
            }
        }
        [HttpGet]
        [Route("Showtags")]
        public async Task<IActionResult> Showtags(int id)
        {

            var categories = _context.Categories
                .FirstOrDefault(p => p.ParentId == id);
            if (categories != null)
            {
                int countofchild = _context.Categories
               .Where(p => p.ParentId == categories.CategoryId).Count();
                if (countofchild > 0)
                {
                    var Catrgorieswithchild = await _context.Categories
                    .Where(p => p.ParentId == id).Select(p => new ViewTags
                    {
                        CategoryId = p.CategoryId,
                        Name = p.Name,


                    }).Distinct().ToListAsync();
                    return Ok(Catrgorieswithchild);

                }
                else
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

                   }).Distinct().ToListAsync();
                    return Ok(Catrgorieswithchild);

                }



            }
            return Ok("");

        }
        
         //search for category by post
        [HttpGet]
        [Route("searchpostcategory")]
        public async Task<ActionResult<IEnumerable<ServicesPost>>> ShowCategory(string cate)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Name.Equals(cate));

            if (category == null)
            {
                return NotFound("category not found");
            }

            var posts = await _context.ServicesPosts.Where(p => p.CategoryId == category.CategoryId).ToListAsync();

            return Ok(posts);
        }


        [HttpGet]
        [Route("searchbytagcategory")]

        public async Task<IActionResult> Search(string tag, int maxResults = 3)
        {
            if (string.IsNullOrEmpty(tag))
            {
                return BadRequest("Tag is required.");
            }

            var result = await _context.ServicesPosts
                        .Where(r => r.SearchTag.ToLower().Contains(tag.ToLower())).Select(p => new PostViewModel
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

                        })
                        .Take(maxResults)
                        .ToListAsync();

            if (!result.Any())
            {
                return NotFound("No records found with the specified tag.");
            }

            return Ok(result);
        }

        // Get all tagssssssssssssssssssssssssssssssss

        [HttpGet]
        [Route("GetCategorytag")]
        public IEnumerable<CategoryModelView> GetCategories(int id)
        {
            var categories = _context.Categories.ToList();
            var rootCategories = categories.Where(c => c.ParentId == id);
            var result = new List<CategoryModelView>();
            foreach (var root in rootCategories)
            {
                GetAllPaths(root, categories, result, id);
            }
            return result.Distinct();
       }

        private void GetAllPaths(Category parent, List<Category> allCategories, List<CategoryModelView> result, int parentId)
        {
            var children = allCategories.Where(c => c.ParentId == parent.CategoryId);
            var childrencount = children.Count();
            if (parent.ParentId == parentId || childrencount == 0)
            {
                if (!result.Any(x => x.Name == parent.Name))
                {
                    result.Add(new CategoryModelView
                    {
                        CategoryId = parent.CategoryId,
                        Name = parent.Name,
                        Image = parent.Image,
                        ParentId = parent.ParentId,
                        Description = parent.Description,
                        Haschild = childrencount > 0,
                    });
                }
            }

            if (children != null && children.Any())
            {
                foreach (var child in children)
                {
                    GetAllPaths(child, allCategories, result, parentId);
                }
            }
        }





    }
}
