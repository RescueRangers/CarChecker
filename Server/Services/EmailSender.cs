using System;
using System.Threading.Tasks;

namespace CarChecker.Server.Services
{
    public class EmailSender : IEmailSender
    {
        public async Task SendEmailAsync(string email, string title, string body)
        {
            throw new NotImplementedException();
        }
    }
}
