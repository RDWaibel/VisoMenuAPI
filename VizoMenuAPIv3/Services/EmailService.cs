using System.Net;
using System.Net.Mail;
using System.Text;
using Mailjet.Client;
using Mailjet.Client.Resources;
using Newtonsoft.Json.Linq;

namespace VizoMenuAPIv3.Services
{
    public class EmailService
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUser;
        private readonly string _smtpPass;
        private readonly string _publicApiKey;
        private readonly string _privateApiKey;

        public EmailService(IConfiguration config)
        {
            _smtpServer = config["smtpServer"];
            _smtpPort = int.Parse(config["smtpPort"]);
            _smtpUser = config["smtpUser"];
            _smtpPass = config["smtpPass"];
            _publicApiKey = config["MJ_APIKEY_PUBLIC"];
            _privateApiKey = config["MJ_APIKEY_PRIVATE"];
        }

        public async Task SendInviteEmailAsync(string toEmail, string inviteLink)
        {
            var mail = new MailMessage
            {
                From = new MailAddress("noreply@vizomenu.com"),
                Subject = "You're invited to VizoMenu!",
                IsBodyHtml = true,
                Body = $@"
                    <p>Hello!</p>
                    <p>You've been invited to join VizoMenu. Click the link below to set your password and activate your account:</p>
                    <p><a href='{inviteLink}'>{inviteLink}</a></p>
                    <p>This link will expire in 24 hours.</p>"
            };

            mail.To.Add(toEmail);

            using var smtp = new SmtpClient(_smtpServer, _smtpPort)
            {
                Credentials = new NetworkCredential(_smtpUser, _smtpPass),
                EnableSsl = true
            };

            try
            {
                await smtp.SendMailAsync(mail);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email failed: {ex.Message}");
                throw;
            }
        }

        public async Task SendBasicEmailAsync(string toEmail, string subject, string htmlBody)
        {
            var mail = new MailMessage
            {
                From = new MailAddress("noreply@vizomenu.com"),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };
            mail.To.Add(toEmail);

            using var smtp = new SmtpClient(_smtpServer, _smtpPort)
            {
                Credentials = new NetworkCredential(_smtpUser, _smtpPass),
                EnableSsl = true
            };

            await smtp.SendMailAsync(mail);
        }

        public async Task SendEmailViaRestAsync(string toEmail, string toName, string subject, string htmlBody, string textBody)
        {
            MailjetClient client = new MailjetClient(_publicApiKey, _privateApiKey);

            var request = new MailjetRequest
            {
                Resource = Send.Resource,
            }
            .Property(Send.FromEmail,"noreply@vizomenu.com")
            .Property(Send.FromName, "VizoMenu Site Admin")
            .Property(Send.Subject, subject)
            .Property(Send.TextPart, textBody)
            .Property(Send.HtmlPart, htmlBody)
            .Property(Send.Recipients, new JArray
            {
                new JObject
                {
                    { "Email", toEmail }
                }
            });

            var response = await client.PostAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorDetails = response.GetErrorMessage();
                var rawContent = response.Content.ToString(Newtonsoft.Json.Formatting.Indented);
                Console.WriteLine($"Mailjet send failed:\nStatusCode: {response.StatusCode}\nError: {errorDetails}\nRaw: {rawContent}");
                throw new Exception($"Mailjet send failed: {response.StatusCode} - {errorDetails}");
            }
        }


    }
}
