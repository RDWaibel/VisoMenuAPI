using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace VizoMenuAPIv3.Data;

public class VizoMenuDbContextFactory : IDesignTimeDbContextFactory<VizoMenuDbContext>
{
    public VizoMenuDbContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("local.settings.json", optional: false, reloadOnChange: true)
            .Build();

        var connectionString = config.GetConnectionString("VizoMenuDb");

        var optionsBuilder = new DbContextOptionsBuilder<VizoMenuDbContext>();

        optionsBuilder.UseSqlServer(connectionString, sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(10),
                errorNumbersToAdd: null);
        });

        return new VizoMenuDbContext(optionsBuilder.Options);
    }
}
