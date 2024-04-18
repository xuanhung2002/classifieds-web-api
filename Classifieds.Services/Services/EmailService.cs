using Classifieds.Services.IServices;
using Common;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace Classifieds.Services.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;

        public EmailService(ILogger<EmailService> logger)
        {
            _logger = logger;
            _logger.LogInformation("Create MailService");
        }
        public async Task Send(string to, string subject, string html)
        {
            var email = new MimeMessage();
            email.Sender = new MailboxAddress(AppSettings.DisplayName, AppSettings.Mail);
            email.From.Add(new MailboxAddress(AppSettings.DisplayName, AppSettings.Mail));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;


            var builder = new BodyBuilder();
            builder.HtmlBody = html;
            email.Body = builder.ToMessageBody();

            // dùng SmtpClient của MailKit
            using var smtp = new MailKit.Net.Smtp.SmtpClient(); // Using finished deleted so as not to slow down the system.

            try
            {
                smtp.Connect(AppSettings.Host, AppSettings.Port, SecureSocketOptions.StartTls);
                smtp.Authenticate(AppSettings.Mail, AppSettings.Password);
                await smtp.SendAsync(email);
            }
            catch (Exception ex)
            {
                // Failed to send an email, email content will be saved in mailssave folder.
                System.IO.Directory.CreateDirectory("mailssave");
                var emailsavefile = string.Format(@"mailssave/{0}.eml", Guid.NewGuid());
                await email.WriteToAsync(emailsavefile);

                _logger.LogInformation("Lỗi gửi mail, lưu tại - " + emailsavefile);
                _logger.LogError(ex.Message);
            }

            smtp.Disconnect(true);

            _logger.LogInformation("send mail to " + to);
        }
    }
}
