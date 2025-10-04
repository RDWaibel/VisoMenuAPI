using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using VizoMenuAPIv3.Data;
using VizoMenuAPIv3.Models;
using VizoMenuAPIv3.Services;

namespace VizoMenuAPIv3.Functions
{
    public  class UserFunctions
    {

        private readonly VizoMenuDbContext _db;
        private readonly JwtService _jwt;
        private readonly EmailService _emailService;

        public UserFunctions(VizoMenuDbContext db, EmailService emailService, JwtService jwt)
        {
            _db = db;
            _emailService = emailService;
            _jwt = jwt;
        }

        [Function("Login")]
        public async Task<HttpResponseData> LoginAsync(
    [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "auth/login")] HttpRequestData req,
    FunctionContext context)
        {
            var logger = context.GetLogger("Login");
            var request = await req.ReadFromJsonAsync<LoginRequest>();
            if (request == null) return req.CreateResponse(HttpStatusCode.BadRequest);

            var user = await _db.Users
                .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Email == request.Email && u.IsEnabled);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return req.CreateResponse(HttpStatusCode.Unauthorized);

            var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
            var token = _jwt.GenerateToken(user, roles);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(new
            {
                token,
                user = new
                {
                    user.Id,
                    user.Email,
                    user.FirstName,
                    user.LastName,
                    roles
                }
            });
            logger.LogInformation($"Logging in user: {user.FirstName} {user.LastName}");
            return response;
        }

        [Function("InviteUser")]
        public async Task<HttpResponseData> InviteUserAsync(
    [HttpTrigger(AuthorizationLevel.Function, "post", Route = "auth/invite")] HttpRequestData req,
    FunctionContext context)
        {
            var logger = context.GetLogger("InviteUser");
            var request = await req.ReadFromJsonAsync<InviteRequest>();
            var response = req.CreateResponse();

            if (request == null || string.IsNullOrEmpty(request.Email))
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                await response.WriteStringAsync("Invalid request.");
                return response;
            }

            // Prevent duplicate invites
            var existing = await _db.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (existing != null)
            {
                response.StatusCode = HttpStatusCode.Conflict;
                await response.WriteStringAsync("User already exists.");
                return response;
            }

            // Generate token
            var token = Guid.NewGuid().ToString();
            var tokenExpires = DateTime.UtcNow.AddHours(24);

            var newUser = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Username = request.Email,
                InviteToken = token,
                InviteTokenExpires = tokenExpires,
                IsActivated = false,
                IsEnabled = true,
                EnteredUTC = DateTime.UtcNow,
                EnteredById = Guid.Empty // or current admin user ID, if available
            };

            _db.Users.Add(newUser);
            await _db.SaveChangesAsync();

            // 🔔 Send invite email
            await _emailService.SendInviteEmailAsync(request.Email, token);

            response.StatusCode = HttpStatusCode.OK;
            await response.WriteStringAsync("Invitation sent.");
            return response;
        }


    }
}
