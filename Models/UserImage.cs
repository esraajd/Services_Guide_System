using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class UserImage
    {
        
            [Key]
            public int UserImageId { get; set; }
            public string FileName { get; set; }
            public string ContentType { get; set; }
            //relation with user
            public string UserId { get; set; }
            public User User { get; set; }



        
    }
}
