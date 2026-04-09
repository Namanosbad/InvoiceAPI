using System.Reflection;
using Microsoft.OpenApi.Models;
using QuestPDF.Infrastructure;
using ServiceCollectionExtensions;

var builder = WebApplication.CreateBuilder(args);

// DEBUG de ambiente (pode remover depois)
Console.WriteLine($"ENVIRONMENT: {builder.Environment.EnvironmentName}");

// Add services to the container.
builder.Services.AddServices(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Invoice API",
        Version = "v1",
        Description = "API interna para gerenciamento de faturas.",
        TermsOfService = new Uri("https://github.com/Namanosbad"),
        Contact = new OpenApiContact
        {
            Name = "Matheus Lima",
            Email = "matheus.limamst@gmail.com",
            Url = new Uri("https://www.linkedin.com/in/matheuslimamst/"),
        }
    });

    var apiXmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var apiXmlPath = Path.Combine(AppContext.BaseDirectory, apiXmlFile);

    if (File.Exists(apiXmlPath))
    {
        c.IncludeXmlComments(apiXmlPath, includeControllerXmlComments: true);
    }

    var applicationXmlFile = "Invoice.API.Application.xml";
    var applicationXmlPath = Path.Combine(AppContext.BaseDirectory, applicationXmlFile);

    if (File.Exists(applicationXmlPath))
    {
        c.IncludeXmlComments(applicationXmlPath);
    }
});

var app = builder.Build();

QuestPDF.Settings.License = LicenseType.Community;

app.UseSwagger();

app.UseSwagger(c =>
{
    c.PreSerializeFilters.Add((swagger, httpReq) =>
    {
        swagger.Servers = new List<Microsoft.OpenApi.Models.OpenApiServer>
        {
            new() { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}" }
        };
    });
});

app.UseAuthorization();

app.MapControllers();

app.Run();
