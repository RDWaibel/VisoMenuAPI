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

    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<Venue> Venues => Set<Venue>();
    public DbSet<Site> Sites => Set<Site>();
}

