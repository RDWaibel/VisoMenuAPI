using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VizoMenuAPIv3.Data;
using VizoMenuAPIv3.Functions;
using VizoMenuAPIv3.Services;

var host = new HostBuilder()
    .ConfigureAppConfiguration(config =>
    {
        config.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
              .AddEnvironmentVariables();
    })
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((context, services) =>
    {
        var connectionString = context.Configuration.GetConnectionString("VizoMenuDb");

        services.AddDbContext<VizoMenuDbContext>(options =>
        options.UseSqlServer(connectionString, sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(10),
                errorNumbersToAdd: null);
        }));

        services.AddSingleton<JwtService>();
        services.AddSingleton<EmailService>();

        services.AddScoped<UserFunctions>();
        services.AddScoped<MenuFunctions>();
        services.AddScoped<ItemImportService>();

    })
    .Build();

    
using (var scope = host.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<VizoMenuDbContext>();
    await DbInitializer.SeedAsync(db);
}

host.Run();
