using System;
using System.Threading.Tasks;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace CarChecker.Server.Services
{
    public class EmailSender : IEmailSender
    {
        private IConfiguration _config;
        private ILogger<EmailSender> _logger;
        
        private string _userName;
        private string _password;
        private string _imapServer;
        private int _port;

        private int _recconectAttempts = 5;

        public EmailSender(IConfiguration config, ILogger<EmailSender> logger)
        {
            GetMailServerConfig(config);
            _logger = logger;
        }

        private void GetMailServerConfig(IConfiguration config)
        {
            _userName = config.GetValue<string>("MailServer:UserName");
            _password = config.GetValue<string>("MailServer:Password");
            _imapServer = config.GetValue<string>("MailServer:SmtpServer");
            _port = config.GetValue<int>("MailServer:SmtpPortSSL");
        }

        private async Task<SmtpClient> ConnectToMailServer()
        {
            try
            {
                _recconectAttempts--;
                
                if (string.IsNullOrEmpty(_userName)) throw new ArgumentNullException(nameof(_userName));
                var client = new SmtpClient();
                await client.ConnectAsync(_imapServer, _port, true).ConfigureAwait(false);
                await client.AuthenticateAsync(_userName, _password).ConfigureAwait(false);
                return client;
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "No user name for email server provided.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when trying to connect to {server}.", _imapServer);
                if (_recconectAttempts < 1) throw;
                _logger.LogInformation("Trying to recconect, {attempts} attempts left", _recconectAttempts);
                return await ConnectToMailServer();
            }
        }

        public async Task SendEmailAsync(string email, string title, string body)
        {
            using var client = await ConnectToMailServer();
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Eko-Myśl", _userName));
            message.To.Add(new MailboxAddress(email, email));
            message.Subject = title;

            message.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = body };

            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
