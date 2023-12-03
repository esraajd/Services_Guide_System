using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Hubs
{
    public class ChatMessage
    {

        public int ChatMessageId { get; set; }
        public string Message { get; set; }

        public DateTime MessageDate { get; set; }
       

        public string SenderId { get; set; }
        
        public User Sender { get; set; }

        public string RecevieId { get; set; }

        public User Recevie { get; set; }

    }
}
