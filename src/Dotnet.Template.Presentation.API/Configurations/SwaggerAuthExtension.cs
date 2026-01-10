// TODO: Re-enable this Swagger authentication configuration once breaking changes from .NET 10 upgrade are resolved.

// using Microsoft.OpenApi;

// namespace Dotnet.Template.Presentation.API.Configurations;

// public static class SwaggerAuthExtension
// {
//     public static IServiceCollection AddSwaggerAuth(this IServiceCollection services)
//     {
//         services.AddSwaggerGen(c =>
//         {
//             c.SwaggerDoc("v1", new OpenApiInfo
//             {
//                 Title = "Dotnet Template API",
//                 Version = "v1"
//             });

//             c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//             {
//                 In = ParameterLocation.Header,
//                 Description = "Please insert token (e.g., Bearer <token>)",
//                 Name = "Authorization",
//                 Type = SecuritySchemeType.Http,
//                 BearerFormat = "JWT",
//                 Scheme = "bearer"
//             });

//             c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
//             {
//                 {
//                     new OpenApiSecurityScheme
//                     {
//                         Reference = new OpenApiReference
//                         {
//                             Type = ReferenceType.SecurityScheme,
//                             Id = "Bearer"
//                         }
//                     },
//                     Array.Empty<string>()
//                 }
//             });
//         });

//         return services;
//     }
// }
