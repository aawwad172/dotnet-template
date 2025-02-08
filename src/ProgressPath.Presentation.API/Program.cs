using ProgressPath.Presentation.API.Configurations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerAuth();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Progress Path API v1"));

app.Run();

/// <summary>
/// The main entry point for the ProgressPath.Presentation.API.
/// This partial class allows the program to be extended with additional methods or configurations.
/// </summary>
public partial class Program { }
