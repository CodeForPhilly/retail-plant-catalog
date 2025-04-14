using System.Data;
using System.Reflection;
using Amazon;
using Amazon.SimpleEmail;
using Dapper;
using FluentLogger;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.OpenApi.Models;
using MySqlConnector;
using Repositories;
using Shared;
using webapi.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v2", new OpenApiInfo { Title = "My API - V2", Version = "v2" });
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Plant Agents Collective API",
        Description = "An API for finding native plants",
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Martin Murphy",
            Url = new Uri("https://savvyotter.com")
        }
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "Bearer",
        In = ParameterLocation.Header,
        Description = "Authorization header using the Bearer scheme."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }, new List<string>() }
    });
    var currentAssembly = Assembly.GetExecutingAssembly().GetName().Name;
    var xmlFilename = $"{currentAssembly}.xml";
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename), true);
});

builder.Services.AddScoped<IDbConnection>((container) =>
{
    var config = container.GetService<IConfiguration>();
    var connStr = config?.GetConnectionString("app");
    return new MySqlConnection(connStr);
});
builder.Services.AddSingleton((container) =>
{
    var config = container.GetService<IConfiguration>();
#pragma warning disable CS8602 // Dereference of a possibly null reference.
    var logDir = config["LogDirectory"] ?? "logs";
#pragma warning restore CS8602 // Dereference of a possibly null reference.
    LogFactory.Init(new ConsoleLogger(), new MaximumFileSizeRoller(logDir, FluentLogger.LogLevel.Trace, () => "Pac Logging", false, 5,5,"pac"));
    var logger = LogFactory.GetLogger();

    logger.Info("Application Initialized: " + DateTime.Now.ToLongDateString());
    return logger;
});

builder.Services.AddTransient<UserRepository, UserRepository>();
builder.Services.AddTransient<VendorRepository, VendorRepository>();
builder.Services.AddTransient<VendorUrlRepository, VendorUrlRepository>();
builder.Services.AddTransient<VendorService, VendorService>();
builder.Services.AddTransient<InviteRepository, InviteRepository>();
builder.Services.AddTransient<ZipRepository, ZipRepository>();
builder.Services.AddTransient<PlantRepository, PlantRepository>();
builder.Services.AddTransient<ApiInfoRepository, ApiInfoRepository>();
builder.Services.AddTransient<PlantCrawler, PlantCrawler>();

builder.Services.AddTransient(c =>
{
    var config = c.GetService<IConfiguration>();
    var access = config.GetValue<string?>("amazonAccess");
    var secret = config.GetValue<string?>("amazonSecret");
    return new AmazonSimpleEmailServiceClient(access, secret, RegionEndpoint.USEast1);
});
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
        options.SlidingExpiration = true;
        options.AccessDeniedPath = "/Forbidden/";
    });
var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.UseDefaultFiles(new DefaultFilesOptions { DefaultFileNames = new string[] { "index.html" } });

app.UseStaticFiles(new StaticFileOptions
{

});
app.UseHttpsRedirection();
app.UseAuthentication();

app.UseAuthorization();
app.UseExceptionHandler(o => { });
app.MapControllers();

app.Run();
