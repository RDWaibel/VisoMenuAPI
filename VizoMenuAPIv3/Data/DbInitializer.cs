using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VizoMenuAPIv3.Models;

namespace VizoMenuAPIv3.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(VizoMenuDbContext db)
        {
            if (!db.Users.Any())
            {
                var superAdminRoleId = Guid.Parse("DE98FEC2-9684-4183-B4BA-7D248DA37BDD");
                var superAdminId = Guid.Parse("4E0878C3-DC9D-4A8E-B51F-8AF10BA9CC3A");

                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Username = "r-waibel",
                    FirstName = "Rob",
                    LastName = "Waibel Jr",
                    Email = "rob@vizomenu.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Trinity1!"), // Added BCrypt package
                    EnteredById = superAdminId,
                    UserRoles = new List<UserRole>
                {
                    new UserRole { RoleId = superAdminRoleId }
                }
                };

                db.Users.Add(user);
                await db.SaveChangesAsync();
            }
        }
    }

}
