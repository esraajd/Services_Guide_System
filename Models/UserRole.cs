using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class UserRole
    {
        [Key]
        public string UserId { get; set; }
        public string UserName { get; set; }
        public bool IsSelected { get; set; }
        public string RoleId { get; set; }
    }
}
