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
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Configuration;

namespace VisoMenuAPI
{
    public class EmailService
    {
        private readonly string _smtpServer;
        private readonly string _smtpPort;
        private readonly string _smtpUser;
        private readonly string _smtpPass;
        private readonly IConfiguration _config;
        public EmailService(IConfiguration config)
        {
            _config = config;
            _smtpServer = _config["MailJet:smtpServer"];
            _smtpPort = _config["MailJet:smtpPort"];
            _smtpUser = _config["MailJet:smtpUser"];
            _smtpPass = _config["MailJet:smtpPass"];
        }

        public async Task SendEmailBasic(Contact_Us _cr)
        {
            var smtpClient = new System.Net.Mail.SmtpClient(_smtpServer, int.Parse(_smtpPort))
            {
                Credentials = new NetworkCredential(_smtpUser, _smtpPass),
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

        public async Task SendEmailViaRestAsync(string toEmail, string toName, string subject, string htmlBody)
        {
            var publicApiKey = _config["MJ_APIKEY_PUBLIC"];
            var privateApiKey = _config["MJ_APIKEY_PRIVATE"];

            if (string.IsNullOrEmpty(publicApiKey) || string.IsNullOrEmpty(privateApiKey))
            {
                Console.WriteLine("Mailjet API keys are missing.");
                return;
            }

            var payload = new
            {
                Messages = new[]
                {
            new
            {
                From = new
                {
                    Email = "noreply@vizomenu.com",
                    Name = "VizoMenu"
                },
                To = new[]
                {
                    new { Email = toEmail, Name = toName }
                },
                Subject = subject,
                HTMLPart = htmlBody
            }
        }
            };

            var json = System.Text.Json.JsonSerializer.Serialize(payload);
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.mailjet.com/v3.1/send")
            {
                Headers =
        {
            Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(Encoding.ASCII.GetBytes($"{publicApiKey}:{privateApiKey}"))
            )
        },
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            using var httpClient = new HttpClient();

            var response = await httpClient.SendAsync(request);
            var result = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Mailjet error: {response.StatusCode} - {result}");
            }
            else
            {
                Console.WriteLine("Email sent successfully via Mailjet REST API.");
            }
        }

    }
}
