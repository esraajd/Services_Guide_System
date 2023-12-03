using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class CategoryPosts
    {

        public int CategoryPostsId { get; set; }

        public int PostId { get; set; }
        public ServicesPost ServicesPost { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

    }
}
