using System.Threading.Tasks;

namespace CarChecker.Server.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string title, string body);
    }
}