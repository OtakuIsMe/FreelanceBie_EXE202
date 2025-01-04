using BE.src.api.shared.Constant;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace BE.src.api.services
{
    public class EmailServ
    {
        private readonly ILogger<EmailServ> logger;

        public EmailServ(ILogger<EmailServ> _logger)
        {
            logger = _logger;
        }

        public async Task<bool> SendVerificationEmail(string toEmail, string subject, string htmlBody)
        {
            var message = new MimeMessage();
            message.Sender = new MailboxAddress(MailKitSender.DisplayName, MailKitSender.Mail);
            message.From.Add(new MailboxAddress(MailKitSender.DisplayName, MailKitSender.Mail));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;

            var builder = new BodyBuilder();
            builder.HtmlBody = htmlBody;
            message.Body = builder.ToMessageBody();

            using var smtp = new MailKit.Net.Smtp.SmtpClient();

            try
            {
                smtp.Connect(MailKitSender.Host, MailKitSender.Port, SecureSocketOptions.StartTls);
                smtp.Authenticate(MailKitSender.Mail, MailKitSender.Password);
                await smtp.SendAsync(message);
                return true;
            }
            catch (Exception ex)
            {
                var emailsavefile = "email_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".eml";
                logger.LogInformation("Failed to send email, save at - " + emailsavefile);
                logger.LogError(ex.Message);
            }

            smtp.Disconnect(true);

            logger.LogInformation("send mail to " + toEmail);
            return false;
        }
    }
}