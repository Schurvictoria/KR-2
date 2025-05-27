using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using FileAnalysisService.Infrastructure.Persistence;
using FileAnalysisService.Application.Services;
using FileAnalysisService.Infrastructure.Adapters;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddControllers();

// Add DbContext
builder.Services.AddDbContext<AnalysisDbContext>(options =>
    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

// Register services
builder.Services.AddScoped<IFileAnalyseService, FileAnalyseService>();

// Configure HTTP clients
builder.Services.AddHttpClient("FileStorer", client =>
{
    var baseUrl = configuration["FileStorer:BaseUrl"]
                  ?? throw new ArgumentNullException("FileStorer:BaseUrl is not configured");
    client.BaseAddress = new Uri(baseUrl);
});

builder.Services.AddHttpClient("WordCloud", client =>
{
    var baseUrl = configuration["WordCloud:BaseUrl"]
                  ?? throw new ArgumentNullException("WordCloud:BaseUrl is not configured");
    client.BaseAddress = new Uri(baseUrl);
});

// Register adapters
builder.Services.AddScoped<IFileStorerAdapter>(sp => 
    new FileStorerHttpAdapter(sp.GetRequiredService<IHttpClientFactory>().CreateClient("FileStorer")));
builder.Services.AddScoped<IWordCloudAdapter>(sp => 
    new WordCloudHttpAdapter(sp.GetRequiredService<IHttpClientFactory>().CreateClient("WordCloud")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "File Analysis Service", Version = "v1" });
});

var app = builder.Build();

// Apply migrations
try
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AnalysisDbContext>();
    db.Database.Migrate();
    Console.WriteLine("Database migrations applied successfully.");
}
catch (Exception ex)
{
    Console.WriteLine($"Error applying migrations: {ex.Message}");
    throw;
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseCors("AllowAll");
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "File Analysis Service v1");
});

app.UseRouting();
app.UseAuthorization();
app.MapControllers();
app.Run();
