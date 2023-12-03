namespace WebApplication1.Models
{
    public class Rate
    {

        public int RateId { get; set; }

        public int Like { get; set; }

        //relation with User
        public User User { get; set; }
        public string UserId { get; set; }

        public ServicesPost ServicesPost  { get; set; }
        public int ServicesPostId { get; set; }
    }
    public class likes
    {
        
        public int Like { get; set;}
        public int Dislike { get; set; }
    }
}
