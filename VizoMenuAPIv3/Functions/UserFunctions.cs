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

        public UserFunctions(VizoMenuDbContext db, JwtService jwt)
        {
            _db = db;
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

            return response;
        }

    }
}
