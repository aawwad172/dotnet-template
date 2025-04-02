using System.Reflection;

using Dotnet.Template.Application;
using Dotnet.Template.Application.CQRS.Commands.Authentication;
using Dotnet.Template.Application.Utilities;
using Dotnet.Template.Domain;
using Dotnet.Template.Infrastructure;
using Dotnet.Template.Presentation.API;
using Dotnet.Template.Presentation.API.Configurations;
using Dotnet.Template.Presentation.API.Middlewares;
using Dotnet.Template.Presentation.API.Models;
using Dotnet.Template.Presentation.API.Routes.Account;

WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks().AddNpgSql(builder.Configuration.GetRequiredSetting("ConnectionStrings:DbConnectionString"));

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerAuth();

builder.Services.AddDomain()
                .AddApplication()
                .AddInfrastructure(builder.Configuration)
                .AddPresentation();

builder.Logging.AddConsole();

WebApplication app = builder.Build();

// Map health check endpoint
app.MapHealthChecks("/health");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Progress Path API v1"));
app.UseMiddleware<ExceptionHandlerMiddleware>();

app.MapGet("/", () => new
{
    message = "Welcome to My API",
    version = "1.0.0",
    links = new
    {
        self = "/",
        docs = "/swagger",
        health = "/health"
    }
}).WithTags("Home");

#region Authentication

app.MapPost("/users", RegisterUser.RegisterRoute).WithTags("Account")
   .Produces<ApiResponse<RegisterUserResult>>(StatusCodes.Status201Created, "application/json")
   .Produces<ApiResponse<IEnumerable<string>>>(StatusCodes.Status400BadRequest, "application/json")
   .Produces<ApiResponse<RegisterUserResult>>(StatusCodes.Status409Conflict, "application/json")
   .Accepts<RegisterUserCommand>("application/json");

#endregion


app.Run();

/// <summary>
/// The main entry point for the Dotnet.Template.Presentation.API.
/// This partial class allows the program to be extended with additional methods or configurations.
/// </summary>
public partial class Program { }
