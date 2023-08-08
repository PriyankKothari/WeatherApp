using Microsoft.AspNetCore.Mvc.Versioning;
using WeatherApp.Application.Services;
using WeatherApp.Application.UseCases.Weather;
using WeatherApp.Infrastructure.Services;
using WeatherApp.ExternalWeatherApi.Client.ApiClients;
using WeatherApp.ExternalWeatherApi.Client.Options;
using WeatherApp.Api.Middlewares;
using AspNetCoreRateLimit;
using Microsoft.OpenApi.Models;

var webApplicationBuilder = WebApplication.CreateBuilder(args);

// Build Services
// Read configuration section "ExternalWeatherApiSettings" into IOptions<ExternalWeatherApiOptions>
webApplicationBuilder.Services.Configure<ExternalWeatherApiOptions>(webApplicationBuilder.Configuration.GetSection("ExternalWeatherApiSettings"));

// Read configuration section "ClientRateLimiting" into IOptions<ClientRateLimitOptions>
webApplicationBuilder.Services.Configure<ClientRateLimitOptions>(webApplicationBuilder.Configuration.GetSection("ClientRateLimiting"));

// For distributed memory cache e.g. Redis
// webApplication.Services.AddDistributedMemoryCache();

// Client Rate Limiting
webApplicationBuilder.Services.AddMemoryCache();
webApplicationBuilder.Services.AddSingleton<IRateLimitConfiguration, CustomRateLimitingConfiguration>();
webApplicationBuilder.Services.AddInMemoryRateLimiting();

// For distributed memory cache e.g. Redis
// webApplication.Services.AddRedisRateLimiting();

// CORS
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

// IHttpClientFactory.
webApplicationBuilder.Services.AddHttpClient();

// Controllers
webApplicationBuilder.Services.AddControllersWithViews();

// API Versioning
webApplicationBuilder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ApiVersionReader = ApiVersionReader.Combine(new UrlSegmentApiVersionReader(), new HeaderApiVersionReader("policies-api-version"));
});

webApplicationBuilder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// API Explorer
webApplicationBuilder.Services.AddEndpointsApiExplorer();
webApplicationBuilder.Services.AddSwaggerGen(policies =>
{
    policies.AddSecurityDefinition("X-API-KEY", new OpenApiSecurityScheme
    {
        Name = "X-API-KEY",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "ApiKeyScheme",
        In = ParameterLocation.Header,
        Description = "ApiKey must appear in header"
    });

    policies.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "X-API-KEY"
                },
                In = ParameterLocation.Header
            },
            new string[]{}
        }
    });
});

// Services
webApplicationBuilder.Services
    .AddScoped<IExternalWeatherApiClient, ExternalWeatherApiClient>()
    .AddScoped<IWeatherService, WeatherService>()
    .AddScoped<ICurrentWeatherHandler, CurrentWeatherHandler>();

// Build Web Application
var webApplication = webApplicationBuilder.Build();

// Configure the HTTP request pipeline.
webApplication.UseSwagger();
webApplication.UseSwaggerUI();

// Https Redirection
webApplication.UseHttpsRedirection();
webApplication.UseStaticFiles();
webApplication.UseRouting();

// Authentication and Authorization
webApplication.UseMiddleware<ApiKeyAuthenticationMiddleware>();
webApplication.UseAuthorization();
webApplication.UseCors("CorsPolicy");

// Client Rate Limiting
webApplication.UseClientRateLimiting();

// Map Controller Route
webApplication.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action}/{string}",
    new { controller = "Weather", action="Index" });

// Map Controllers
webApplication.MapControllers();

webApplication.MapFallbackToFile("index.html"); ;

webApplication.Run();