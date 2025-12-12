using MediatR;
using Microsoft.Extensions.Logging;
using MeuRh.Application.DTOs;
using MeuRh.Application.Services;
using MeuRh.Domain.Exceptions;
using MeuRh.Domain.Interfaces.Service;

namespace MeuRh.Application.Commands.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserResponseDto>
{
    private readonly IUserService _userService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<CreateUserCommandHandler> _logger;

    public CreateUserCommandHandler(
        IUserService userService,
        IPasswordHasher passwordHasher,
        ILogger<CreateUserCommandHandler> logger)
    {
        _userService = userService;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task<UserResponseDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando cadastro de novo usuário com e-mail: {Email}", request.Email);

        try
        {
            var passwordHash = _passwordHasher.HashPassword(request.Password);
            var user = await _userService.CreateUserAsync(request.Name, request.Email, passwordHash, cancellationToken);

            _logger.LogInformation("Usuário cadastrado com sucesso. Id: {UserId}, Email: {Email}", user.Id, user.Email);

            return new UserResponseDto(user.Id, user.Name, user.Email, user.CreatedAt);
        }
        catch (BusinessRuleException)
        {
            _logger.LogWarning("Tentativa de cadastro com e-mail já existente: {Email}", request.Email);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao persistir usuário no banco de dados. Email: {Email}", request.Email);
            throw;
        }
    }
}

