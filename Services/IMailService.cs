using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public interface IMailService
    {
        public Task SendEmailAsync(MailRequest mailRequest);
    }
}
