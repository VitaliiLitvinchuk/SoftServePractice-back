using System.Reflection;
using Application.Common.Behaviours;
using Application.Common.Interfaces.Services;
using Application.Common.Services;
using FluentValidation;
using Hangfire;
using Hangfire.PostgreSql;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace Application;

public static class ConfigureApplication
{
    public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddServices();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly(), ServiceLifetime.Scoped, includeInternalTypes: true);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

        services.ConfigureHangfire(configuration);
    }

    private static void AddServices(this IServiceCollection services)
    {
        services.AddSingleton<IFileService, FileService>();
        services.AddSingleton<IJwtService, JwtService>();
        services.AddSingleton<IHashService, HashService>();

        services.AddScoped<IHangfireSessionService, HangfireSessionService>();
    }

    private static void ConfigureHangfire(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHangfire(config =>
            config.UsePostgreSqlStorage(options =>
            {
                options.UseNpgsqlConnection(configuration.GetConnectionString("DefaultConnection"));
            }));

        services.AddHangfireServer();
    }

    public static void UseHangfireDashboardAndJobs(this WebApplication app)
    {
        app.UseHangfireDashboard();
        app.MapHangfireDashboard();

        RecurringJob.AddOrUpdate<IHangfireSessionService>(
            "change-session-state",
            service => service.ChangeSessionState(),
            Cron.Minutely);
    }

    public static string UploadsDir { get; private set; } = "";
    public static IApplicationBuilder ConfigureStaticFiles(this IApplicationBuilder app)
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
}
