using System.Reflection;
using Application.Common.Behaviours;
using Application.Common.Interfaces.Services;
using Application.Common.Services;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace Application;

public static class ConfigureApplication
{
    public static void AddApplication(this IServiceCollection services)
    {
        AddServices(services);

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly(), ServiceLifetime.Scoped, includeInternalTypes: true);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
    }

    public static string UploadsDir { get; private set; } = "";
    public static IApplicationBuilder UseAppStaticFiles(this IApplicationBuilder app)
    {
        UploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

        if (!Directory.Exists(UploadsDir))
            Directory.CreateDirectory(UploadsDir);

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(UploadsDir),
            RequestPath = "/files"
        });

        return app;
    }

    private static void AddServices(this IServiceCollection services)
    {
        services.AddSingleton<IFileService, FileService>();
        services.AddSingleton<IJwtService, JwtService>();
        services.AddSingleton<IHashService, HashService>();
    }
}
