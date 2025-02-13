using Dotnet.Template.Application;
using Dotnet.Template.Domain;
using Dotnet.Template.Infrastructure;
using Dotnet.Template.Presentation.API.Configurations;
using Dotnet.Template.Presentation.API.Middlewares;

WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerAuth();

builder.Services.AddDomain()
                .AddApplication()
                .AddInfrastructure();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Progress Path API v1"));
app.UseMiddleware<ExceptionHandlerMiddleware>();

app.Run();

/// <summary>
/// The main entry point for the Dotnet.Template.Presentation.API.
/// This partial class allows the program to be extended with additional methods or configurations.
/// </summary>
public partial class Program { }
