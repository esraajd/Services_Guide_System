using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class Category
    {
        public int CategoryId { get; set; }

        [Required]
        public string Name { get; set; }
        public string Image { get; set; }


        // Foreign key relationship to parent category    
        public int? ParentId { get; set; }

        
        public string Description { get; set; }

        // Navigation property to parent category    
        public virtual Category Parent { get; set; }

        // Navigation property to child categories    
        public virtual ICollection<Category> Children { get; set; }

    }
    public class CategoryModelView
    {
        public int CategoryId { get; set; }

        public string Name { get; set; }
        public string Image { get; set; }
        public int? ParentId { get; set; }


        public string Description { get; set; }
        public bool Haschild { get; set; }
        public List<CategoryModelView> Children { get; set; }

    }
    public class ViewTags
    {
        public int CategoryId { get; set; }

        public string Name { get; set; }
    }
    public class Treeview
    {
        public List<Category> treeview { get; set; }
    }
}
