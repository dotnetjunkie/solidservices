using Microsoft.OpenApi.Models;
using SimpleInjector;
using WebCoreMinimalApiService;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// Open http://localhost:5132/swagger/ to browse the API.
services.AddEndpointsApiExplorer();
services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "SOLID Services API",
    });
    var appDataPath = AppDomain.CurrentDomain.BaseDirectory;

    // The XML comment files are copied using a post-build event (see project settings / Build Events).
    options.IncludeXmlDocumentationFromDirectory(appDataPath);

    // Optional but useful: this includes the summaries of the command and query types in the operations.
    options.IncludeMessageSummariesFromXmlDocs(appDataPath);
});

var container = new Container();

services.AddSimpleInjector(container, options =>
{
    options.AddAspNetCore();
});

Bootstrapper.Bootstrap(container);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapCommands("/api/commands/{0}", container, Bootstrapper.GetKnownCommandTypes());
app.MapQueries("/api/queries/{0}", container, Bootstrapper.GetKnownQueryTypes());

app.Run();