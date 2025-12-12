using MediatR;
using Microsoft.Extensions.Logging;
using MeuRh.Application.Services;
using MeuRh.Domain.Exceptions;
using MeuRh.Domain.Interfaces.Service;

namespace MeuRh.Application.Commands.RegisterUser;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand>
{
    private readonly IUserService _userService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<RegisterUserCommandHandler> _logger;

    public RegisterUserCommandHandler(
        IUserService userService,
        IPasswordHasher passwordHasher,
        ILogger<RegisterUserCommandHandler> logger)
    {
        _userService = userService;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando registro de novo usuário com e-mail: {Email}", request.Email);

        try
        {
            var passwordHash = _passwordHasher.HashPassword(request.Password);
            var user = await _userService.CreateUserAsync(request.Name, request.Email, passwordHash, cancellationToken);

            _logger.LogInformation("Usuário registrado com sucesso. Id: {UserId}, Email: {Email}", user.Id, user.Email);
        }
        catch (BusinessRuleException)
        {
            _logger.LogWarning("Tentativa de registro com e-mail já existente: {Email}", request.Email);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao persistir usuário no banco de dados. Email: {Email}", request.Email);
            throw;
        }
    }
}

