using FluentValidation;
using MediatR;
using MeuRh.Application.Behaviors;
using MeuRh.Application.Commands.CreateUser;
using Microsoft.Extensions.DependencyInjection;

namespace MeuRh.IoC.Extensions;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(CreateUserCommand).Assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(typeof(CreateUserCommandValidator).Assembly);

        return services;
    }
}

