using MeuRh.IoC.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MeuRh.IoC;

public static class DependencyInjection
{
    public static IServiceCollection AddIoC(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApplication();
        services.AddPersistence(configuration);
        services.AddInfrastructure();
        services.AddAuthentication(configuration);
        services.AddSwagger();

        return services;
    }
}

