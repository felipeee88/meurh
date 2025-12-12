using Microsoft.Extensions.DependencyInjection;
using MeuRh.Application.Services;
using MeuRh.Domain.Interfaces.Repository;
using MeuRh.Domain.Interfaces.Service;
using MeuRh.Domain.Services;
using MeuRh.Infra.Repositories;
using MeuRh.Infra.Services;

namespace MeuRh.IoC.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenService, TokenService>();

        return services;
    }
}

