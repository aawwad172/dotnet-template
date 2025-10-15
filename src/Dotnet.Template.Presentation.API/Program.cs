using Dotnet.Template.Application;
using Dotnet.Template.Application.CQRS.Commands.Authentication;
using Dotnet.Template.Application.Utilities;
using Dotnet.Template.Domain;
using Dotnet.Template.Domain.Entities.Authentication;
using Dotnet.Template.Infrastructure;
using Dotnet.Template.Presentation.API;
using Dotnet.Template.Presentation.API.Configurations;
using Dotnet.Template.Presentation.API.Middlewares;
using Dotnet.Template.Presentation.API.Models;
using Dotnet.Template.Presentation.API.Routes.Authentication;

using RefreshToken = Dotnet.Template.Presentation.API.Routes.Authentication.RefreshToken;

WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks()
                .AddNpgSql(builder.Configuration.GetRequiredSetting("ConnectionStrings:DbConnectionString"));

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerAuth();

builder.Services.AddDomain()
                .AddApplication()
                .AddInfrastructure(builder.Configuration)
                .AddPresentation(builder.Configuration);

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

WebApplication app = builder.Build();

// Map health check endpoint
app.MapHealthChecks("/health");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.DocumentTitle = "Dotnet Template API");
}

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Dotnet Template API v1");
    c.DocumentTitle = "Dotnet Template API";
});
app.UseMiddleware<ExceptionHandlerMiddleware>();
app.UseMiddleware<JwtMiddleware>();

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

app.MapPost("/users/register", RegisterUser.RegisterRoute)
    .WithTags("Authentication")
   .Produces<ApiResponse<RegisterUserResult>>(StatusCodes.Status201Created, "application/json")
   .Produces<ApiResponse<IEnumerable<string>>>(StatusCodes.Status400BadRequest, "application/json")
   .Produces<ApiResponse<RegisterUserResult>>(StatusCodes.Status409Conflict, "application/json")
   .Accepts<RegisterUserCommand>("application/json");

app.MapPost("/users/login", Login.RegisterRoute)
    .WithTags("Authentication")
   .Produces<ApiResponse<LoginResult>>(StatusCodes.Status200OK, "application/json")
   .Produces<ApiResponse<IEnumerable<string>>>(StatusCodes.Status400BadRequest, "application/json")
   .Produces<ApiResponse<LoginResult>>(StatusCodes.Status401Unauthorized, "application/json")
   .Accepts<LoginCommand>("application/json");

app.MapPost("/users/refresh-token", RefreshToken.RegisterRoute)
    .WithTags("Authentication")
        .RequireAuthorization(PolicyNames.UserOnly)
   .Produces<ApiResponse<RefreshTokenCommandResult>>(StatusCodes.Status200OK, "application/json")
   .Produces<ApiResponse<IEnumerable<string>>>(StatusCodes.Status400BadRequest, "application/json")
   .Produces<ApiResponse<RefreshTokenCommandResult>>(StatusCodes.Status401Unauthorized, "application/json")
   .Accepts<RefreshTokenCommand>("application/json");

app.MapPost("/users/logout", Logout.RegisterRoute)
    .WithTags("Authentication")
    .RequireAuthorization("UserPolicy")
   .Produces<ApiResponse<LogoutCommandResult>>(StatusCodes.Status200OK, "application/json")
   .Produces<ApiResponse<IEnumerable<string>>>(StatusCodes.Status400BadRequest, "application/json")
   .Produces<ApiResponse<LogoutCommandResult>>(StatusCodes.Status401Unauthorized, "application/json")
   .Accepts<LogoutCommand>("application/json");

#endregion

app.Run();

/// <summary>
/// The main entry point for the Dotnet.Template.Presentation.API.
/// This partial class allows the program to be extended with additional methods or configurations.
/// </summary>
public partial class Program { }
