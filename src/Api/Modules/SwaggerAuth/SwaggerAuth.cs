using Microsoft.OpenApi.Models;
using System.Reflection;

namespace Api.Modules.SwaggerAuth;

public static class SwaggerAuth
{
    public static IServiceCollection UseSwaggerGen(this IServiceCollection services)
    {
        var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
        services.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition("Bearer",
                new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme.",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer"
                });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement{
            {
                new OpenApiSecurityScheme{
                    Reference = new OpenApiReference{
                        Id = "Bearer",
                        Type = ReferenceType.SecurityScheme
                    }
                },new List<string>()
            }
            });
        });

        return services;
    }
}
