using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class PostViewModel
    {
        public int ServicesPostId { set; get; }

        public List<string> SearchTag { set; get; }

        public string Title { set; get; }

        public string Description { set; get; }

        public string UserId { get; set; }

        public string FullName { get; set; }

        public string Image { get; set; }
       

        public int Like { get; set; }
        public int DisLike { get; set; }


    }


    public class PostCreateViewModel
    {

        public string Title { set; get; }

        public string Description { set; get; }
        
        public int CategoryId { get; set; }
       public List<string> Tags { get; set; }


    }

    


    public class PostsChildViewModel
    {

        public List<CategoryModelView> categories { get; set; }

        public List<PostViewModel> Posts { get; set; }
    
  
    }
}
