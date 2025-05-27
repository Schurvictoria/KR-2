using Microsoft.OpenApi.Models;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();

        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "API Gateway", Version = "v1" });
            c.OperationFilter<SwaggerFileOperationFilter>();
        });

        services.AddHttpClient("FileStorage", client =>
        {
            var baseUrl = _configuration["FileStorageService:BaseUrl"]
                          ?? "http://file-storing:15001/";
            client.BaseAddress = new Uri(baseUrl);
        });

        services.AddHttpClient("FileAnalysis", client =>
        {
            var baseUrl = _configuration["FileAnalysisService:BaseUrl"]
                          ?? "http://file-analysis:4244/";
            client.BaseAddress = new Uri(baseUrl);
        });
    }

    public void Configure(WebApplication app, IWebHostEnvironment env)
    {
        //if (env.IsDevelopment())
        //{
        //    app.UseDeveloperExceptionPage();
        //}
        //else
        //{
        //    app.UseExceptionHandler("/error");
        //}

        app.UseCors("AllowAll");

        app.UseRouting();

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Gateway v1");
            c.RoutePrefix = "";
        });

        app.UseAuthorization();

        app.MapControllers();
    }
}