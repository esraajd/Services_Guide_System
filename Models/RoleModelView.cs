using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class RoleModelView
    {
        [Key]
        public string RoleId { get; set; }
        public string RoleName { get; set; }
    }
}
