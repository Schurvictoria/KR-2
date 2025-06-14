using FileStoringService.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);

var app = builder.Build();

const int maxRetries = 10;
const int delayMs = 5000;
int attempt = 0;
while (true)
{
    try
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<FileDbContext>();
        db.Database.EnsureCreated();
        Console.WriteLine("Database is ready");
        break;
    }
    catch (Exception ex)
    {
        attempt++;
        if (attempt >= maxRetries)
        {
            Console.Error.WriteLine($"Failed to initialize database after {maxRetries} attempts: {ex.Message}\n");
            throw;
        }
        Console.WriteLine($"Database unavailable, retry {attempt} of {maxRetries} in {delayMs / 1000} seconds: {ex.Message}\n");
        Thread.Sleep(delayMs);
    }
}

startup.Configure(app, app.Environment);

app.Run();
