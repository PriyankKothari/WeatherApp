using Microsoft.AspNetCore.Mvc.Versioning;
using WeatherApp.Application.Services;
using WeatherApp.Application.UseCases.Weather;
using WeatherApp.Infrastructure.Services;

var webApplicationBuilder = WebApplication.CreateBuilder(args);

// Build Services
// Add CORS
webApplicationBuilder.Services.AddCors(policy =>
{
    policy.AddPolicy("CorsPolicy", options =>
    {
        options
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Controllers
webApplicationBuilder.Services.AddControllersWithViews();

// API Explorer
webApplicationBuilder.Services.AddEndpointsApiExplorer();
webApplicationBuilder.Services.AddSwaggerGen();

// API Versioning
webApplicationBuilder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ApiVersionReader = ApiVersionReader.Combine(new UrlSegmentApiVersionReader(), new HeaderApiVersionReader("x-api-version"));
});

webApplicationBuilder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// Services
webApplicationBuilder.Services
    .AddScoped<IWeatherService, WeatherService>()
    .AddScoped<ICurrentWeatherHandler, CurrentWeatherHandler>();

// Build Web Application
var webApplication = webApplicationBuilder.Build();

// Configure the HTTP request pipeline.
if (webApplication.Environment.IsDevelopment())
{
    webApplication.UseSwagger();
    webApplication.UseSwaggerUI();
}
webApplication.UseHttpsRedirection();
webApplication.UseAuthorization();
webApplication.UseCors("CorsPolicy");

//webApplication.UseHttpsRedirection();
//webApplication.UseStaticFiles();
//webApplication.UseRouting();

webApplication.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action}/{string}",
    new { controller = "Weather", action="Index" });

webApplication.MapControllers();

webApplication.MapFallbackToFile("index.html"); ;

webApplication.Run();
