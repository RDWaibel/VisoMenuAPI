using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using VizoMenuAPIv3.Services;
using static VizoMenuAPIv3.Models.email;

namespace VizoMenuAPIv3.Functions
{
    public class EmailFunctions

    {
        private readonly EmailService _emailService;

        public EmailFunctions(EmailService emailService)
        {
            _emailService = emailService;
        }

        [Function("SendEmail")]
        public async Task<HttpResponseData> SendEmailAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "email/send")] HttpRequestData req,
            FunctionContext context)
        {
            var logger = context.GetLogger("SendEmail");

            var request = await req.ReadFromJsonAsync<SendEmailRequest>();
            if (request == null || string.IsNullOrWhiteSpace(request.To))
                return req.CreateResponse(HttpStatusCode.BadRequest);

            try
            {
                await _emailService.SendEmailViaRestAsync(request.To, request.ToName, request.Subject, request.textBody, request.htmlBody);

                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteStringAsync("Email sent.");
                return response;
            }
            catch (Exception ex)
            {
                logger.LogError($"Email sending failed: {ex.Message}");
                var error = req.CreateResponse(HttpStatusCode.InternalServerError);
                await error.WriteStringAsync("Failed to send email.");
                return error;
            }
        }

    }
}
