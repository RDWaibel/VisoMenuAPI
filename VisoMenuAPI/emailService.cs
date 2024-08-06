using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using VisoMenuAPI.data;
using System.Net.Mail;
using Mailjet.Client;
using Mailjet.Client.Resources;
using Newtonsoft.Json.Linq;

using static VisoMenuAPI.controllers.emailService;
using System.Net;

namespace VisoMenuAPI
{
    public class EmailService
    {
        private readonly string _smtpServer;
        private readonly string _smtpPort;
        private readonly string _smtpUser;
        private readonly string _smtpPass;

        public EmailService()
        {
            _smtpServer = System.Environment.GetEnvironmentVariable("smtpServer");
            _smtpPort = System.Environment.GetEnvironmentVariable("smtpPort");
            _smtpUser = System.Environment.GetEnvironmentVariable("smtpUser");
            _smtpPass = System.Environment.GetEnvironmentVariable("smtpPass");
        }

        public async Task SendEmailBasic(Contact_Us _cr)
        {
            var mj1 = Environment.GetEnvironmentVariable("MJ_APIKEY_PUBLIC");
            var mj2 = Environment.GetEnvironmentVariable("MJ_APIKEY_PRIVATE");

            var smtpClient = new System.Net.Mail.SmtpClient("in.mailjet.com", 587)
            {
                Credentials = new NetworkCredential(mj1, mj2),
                EnableSsl = true
            };
            var mailMessage = new MailMessage
            {
                From = new MailAddress("rob@vizomenu.com"),
                Subject = "Vizomenu Contact Form",
                IsBodyHtml = true,
                Body = $"<b>NAME </b> {_cr.ContactName}<br/>  <b>EMAIL </b> {_cr.email} <br/> {_cr.message}"
            };
            mailMessage.To.Add("anthony@vizomenu.com");
            mailMessage.To.Add("rob@vizomenu.com");
            mailMessage.To.Add("lroyhale@yahoo.com");
            mailMessage.To.Add("soccerkidc130@aol.com");

            try
            {
                // Send the email
                smtpClient.Send(mailMessage);
            }
            catch(SmtpException  ex) 
            {
                Console.WriteLine($"SMTP error occurred: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}
