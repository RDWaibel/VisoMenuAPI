using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using VizoMenuAPIv3.Models;

namespace VizoMenuAPIv3.Data;

public class VizoMenuDbContext : DbContext
{
    public VizoMenuDbContext(DbContextOptions<VizoMenuDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<Venue> Venues => Set<Venue>();
    public DbSet<VenueHour> VenueHours { get; set; }
    public DbSet<Site> Sites { get; set; }
    public DbSet<Menu> Menus { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserRole>()
            .HasKey(ur => new { ur.UserId, ur.RoleId });

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId);

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId);

        modelBuilder.Entity<User>()
            .HasOne(u => u.EnteredBy)
            .WithMany()
            .HasForeignKey(u => u.EnteredById)
            .OnDelete(DeleteBehavior.NoAction); // 👈 Key part


        foreach (var foreignKey in modelBuilder.Model.GetEntityTypes()
            .SelectMany(e => e.GetForeignKeys())
            .Where(fk => fk.PrincipalEntityType.ClrType == typeof(User)))
        {
            foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
        }

        //Seeds as required
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = Guid.Parse("DE98FEC2-9684-4183-B4BA-7D248DA37BDD"), Name = "SuperAdmin" },
            new Role { Id = Guid.Parse("4B963C3C-7D9D-4787-BE0F-404E2B4A1BDF"), Name = "Admin" },
            new Role { Id = Guid.Parse("E1C88B18-1199-4A78-9B4A-11432B077712"), Name = "OrgAdmin" },
            new Role { Id = Guid.Parse("F8181BE3-9021-4415-BE9F-D7FE3664329A"), Name = "VenueAdmin" },
            new Role { Id = Guid.Parse("49CBD11B-3BFC-4060-91F5-DC54D43F9848"), Name = "SiteAdmin" },
            new Role { Id = Guid.Parse("91AF38D4-7D3C-4286-B35D-184929DD4AE8"), Name = "SiteUser" }
        );

        modelBuilder.Entity<Venue>(entity =>
        {
            entity.ToTable("Venues");
            entity.HasKey(v => v.Id);

            entity.Property(v => v.VenueName).IsRequired().HasMaxLength(200);
            entity.Property(v => v.Location).HasMaxLength(300);

            entity.HasOne(v => v.Organization)
                  .WithMany(o => o.Venues)
                  .HasForeignKey(v => v.OrganizationId);

            entity.HasMany(v => v.Hours)
                  .WithOne(h => h.Venue)
                  .HasForeignKey(h => h.VenueId);
        });

        modelBuilder.Entity<VenueHour>(entity =>
        {
            entity.ToTable("VenueHours");
            entity.HasKey(h => h.Id);

            entity.Property(h => h.DayOfWeek).IsRequired();
            entity.Property(h => h.OpenTime);
            entity.Property(h => h.CloseTime);
            entity.Property(h => h.IsClosed).IsRequired();
        });
        modelBuilder.Entity<Site>()
            .HasOne(s => s.Venue)
            .WithMany(v => v.Sites)
            .HasForeignKey(s => s.VenueId);
        }
}


