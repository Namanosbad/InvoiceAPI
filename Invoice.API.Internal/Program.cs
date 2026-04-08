using System.Reflection;
using Microsoft.OpenApi.Models;
using ServiceCollectionExtensions;

var builder = WebApplication.CreateBuilder(args);

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

    var applicationXmlFile = "Invoice.Application.xml";
    var applicationXmlPath = Path.Combine(AppContext.BaseDirectory, applicationXmlFile);
    if (File.Exists(applicationXmlPath))
    {
        c.IncludeXmlComments(applicationXmlPath);
    }
});

var app = builder.Build();

// Configure the HTTP request pipeline.
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Invoice API v1");
    });

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
