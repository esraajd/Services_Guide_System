using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace WebApplication1.Models
{
    public class ServicesPost
    {
        public int ServicesPostId { set; get; }

        [Required]
        public string SearchTag { set; get; }
        [Required]
        public string SearchTagid { set; get; }

        [Required]
        public string Title { set; get; }
       
        [Required]
        public string Description { set; get; }


        //With Category
        public int CategoryId { get; set; }
        
        
        public virtual Category Categories { get; set; }
        
        
        //With User
        public User User { get; set; }
        
        public string UserId { get; set; }


       

    }
    public class ServicePostModelView
    {
        public int ServicesPostId { set; get; }

        public List<string> SearchTag { set; get; }
        public List<string> SearchTagid { set; get; }



        public string Title { set; get; }

        public string Description { set; get; }
        public int CategoryId { get; set; }


        


    }
}
