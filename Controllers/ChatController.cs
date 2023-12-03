using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using MimeKit;
using MoreLinq;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApplication1.Data;
using WebApplication1.Hubs;
using WebApplication1.Migrations;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IHubContext<ChatHub, IChatClient> _chatHub;
        private readonly ApplicationDbContext _context;
        private UserManager<User> userManager;
        private static int CounterChat;
        public ChatController(IHubContext<ChatHub, IChatClient> chatHub, ApplicationDbContext context)
        {
            _chatHub = chatHub;
            _context = context;
        }
        public string Imgurl = "https://192.168.77.222:45459/images/";

        public class ChatViewModel
        {
            public string ChatWith { get; set; }
            public string PersonalImage { get; set; }
            public string chatwithId { get; set; }
            public DateTime MessageDate { get; set; }



        }
        public class MessageViewModel
        {
            public string ChatWith { get; set; }
            public string Message { get; set; }
            public string receiveid { get; set; }
            public string senderid { get; set; }
            public string date { get; set; }
            public string time { get; set; }
            public string type { get; set; }
            public int ChatMessageId { get; set; }
            public string PersonalImage { get; set; }

        }


        [HttpPost("messages")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]


        public async Task<ActionResult> Post(ChatMessage message)
        {
            await _chatHub.Clients.All.ReceiveMessage(message);
            string senderid = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            message.SenderId = senderid;
            message.MessageDate = DateTime.Now;
            _context.ChatMessages.Add(message);
            _context.SaveChanges();
            CounterChat = CounterChat + 1;
            MessageViewModel message1 = new MessageViewModel
            {
                ChatMessageId = message.ChatMessageId,
                ChatWith = _context.User.FirstOrDefault(u=>u.Id == message.RecevieId)?.FirstName + " " + _context.User.FirstOrDefault(u => u.Id == message.RecevieId).LastName,
                receiveid = message.RecevieId,
                senderid = message.SenderId == senderid ? senderid : message.RecevieId,
                Message = message.Message,
                type ="" ,
                PersonalImage = Imgurl + _context.UserImage.FirstOrDefault(img => img.UserId == senderid).FileName,
                date = message.MessageDate.Year.ToString() + "/" + message.MessageDate.Month.ToString() + "/" + message.MessageDate.Day.ToString(),
                time = message.MessageDate.Hour.ToString() + ":" + message.MessageDate.Minute.ToString(),


            };

            return Ok(message1);
        }


        //[HttpGet("showchats")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //public async Task<ActionResult<List<ChatViewModel>>> Show()
        //{
        //    string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        //    // Retrieve all chat messages where the current user is the sender
        //    if (_context.ChatMessages
        //        .FirstOrDefault(msg => msg.SenderId == userId)?.SenderId == userId)
        //    {
        //        var messages = _context.ChatMessages
        //       .Where(msg => msg.SenderId == userId).OrderByDescending(msg => msg.MessageDate)
        //       .Select(msg => new ChatViewModel
        //       {
        //           ChatWith = msg.Recevie.FirstName + " " + msg.Recevie.LastName,
        //           chatwithId = msg.RecevieId,
        //           PersonalImage = Imgurl + _context.UserImage.FirstOrDefault(img => img.UserId == msg.RecevieId).FileName,
        //           MessageDate = msg.MessageDate
        //       }).DistinctBy(x => x.chatwithId).ToList();
        //       return Ok(messages.OrderByDescending(m => m.MessageDate));


        //    }
        //    else if (_context.ChatMessages
        //        .FirstOrDefault(msg => msg.RecevieId == userId)?.RecevieId == userId){
        //        var messages = _context.ChatMessages
        //       .Where(msg => msg.RecevieId == userId).OrderByDescending(msg => msg.MessageDate)
        //       .Select(msg => new ChatViewModel
        //       {
        //           ChatWith = msg.Sender.FirstName + " " + msg.Sender.LastName,
        //           chatwithId = msg.SenderId,
        //           PersonalImage = Imgurl + _context.UserImage.FirstOrDefault(img => img.UserId == msg.SenderId).FileName,
        //           MessageDate = msg.MessageDate
        //       }).DistinctBy(x => x.chatwithId).ToList();
        //        return Ok(messages.OrderByDescending(m => m.MessageDate));

        //    }


        //    CounterChat = 0;
        //    return Ok();

        //    }
        [HttpGet("showchats")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<List<ChatViewModel>>> Show()
        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            var messagesSender = _context.ChatMessages
                .Where(msg => msg.SenderId == userId)
                .OrderByDescending(msg => msg.MessageDate)
                .Select(msg => new ChatViewModel
                {
                    ChatWith = msg.Recevie.FirstName + " " + msg.Recevie.LastName,
                    chatwithId = msg.RecevieId,
                    PersonalImage = Imgurl + _context.UserImage.FirstOrDefault(img => img.UserId == msg.RecevieId).FileName,
                    MessageDate = msg.MessageDate
                }).DistinctBy(x => x.chatwithId).ToList();

            var messagesReceiver = _context.ChatMessages
                .Where(msg => msg.RecevieId == userId)
                .OrderByDescending(msg => msg.MessageDate)
                .Select(msg => new ChatViewModel
                {
                    ChatWith = msg.Sender.FirstName + " " + msg.Sender.LastName,
                    chatwithId = msg.SenderId,
                    PersonalImage = Imgurl + _context.UserImage.FirstOrDefault(img => img.UserId == msg.SenderId).FileName,
                    MessageDate = msg.MessageDate
                }).DistinctBy(x => x.chatwithId).ToList();

            var messages = messagesSender.Concat(messagesReceiver).OrderByDescending(m => m.MessageDate);
            CounterChat = 0;

            return Ok(messages.DistinctBy(x => x.chatwithId).ToList());
        }




        [HttpGet("showmessages")]

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<List<MessageViewModel>>> GetMessages(string chatwithid)
        {
            // Get the sender's user ID from the JWT bearer token
            var senderId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(senderId))
            {
                return Unauthorized();
            }
            List<MessageViewModel> AllChat = new List<MessageViewModel>();

            var messages = _context.ChatMessages
            .Where(m => m.SenderId == senderId && m.RecevieId == chatwithid || m.SenderId == chatwithid && m.RecevieId == senderId)
            .Select(m => new MessageViewModel
            {
                ChatMessageId = m.ChatMessageId,
                ChatWith = m.Recevie.FirstName + " " + m.Recevie.LastName,
                receiveid = m.RecevieId,
                senderid = m.SenderId==senderId? senderId:chatwithid,
                Message = m.Message,
                type = m.SenderId == senderId ? "" : "other",
                PersonalImage = m.SenderId == chatwithid ? Imgurl + _context.UserImage.FirstOrDefault(img => img.UserId == chatwithid).FileName: Imgurl + _context.UserImage.FirstOrDefault(img => img.UserId ==senderId).FileName,
                date = m.MessageDate.Year.ToString() + "/" + m.MessageDate.Month.ToString() + "/" + m.MessageDate.Day.ToString(),
                time = m.MessageDate.Hour.ToString() + ":" + m.MessageDate.Minute.ToString(),

            });


            return Ok(messages);

        }
           

        [HttpGet]
        [Route("Notificationchat")]

        public async Task<ActionResult> Notificationchat()
        {
            Note c = new Note
            {
                Count = CounterChat

            };
            return Ok(c);
        }



    }
}
