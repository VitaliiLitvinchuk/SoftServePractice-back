using Application;
using Infrastructure;

namespace Api.Dependencies;

public static class Depedency
{
    public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddInfrasctructure(configuration);

        services.AddApplication();

        return services;
    }
}
