using MediatR;
using Microsoft.Extensions.Logging;
using MeuRh.Application.DTOs;
using MeuRh.Application.Services;
using MeuRh.Domain.Exceptions;
using MeuRh.Domain.Interfaces.Service;

namespace MeuRh.Application.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponseDto>
{
    private readonly IUserService _userService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(
        IUserService userService,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        ILogger<LoginCommandHandler> logger)
    {
        _userService = userService;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<LoginResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando autenticação do usuário com e-mail: {Email}", request.Email);

        var user = await _userService.GetUserByEmailAsync(request.Email, cancellationToken);
        if (user == null)
        {
            _logger.LogWarning("Tentativa de login com e-mail não encontrado: {Email}", request.Email);
            throw new BusinessRuleException("Usuário ou senha inválidos.");
        }

        var isPasswordValid = _passwordHasher.VerifyPassword(request.Password, user.PasswordHash);
        if (!isPasswordValid)
        {
            _logger.LogWarning("Tentativa de login com senha inválida para o usuário: {Email}", request.Email);
            throw new BusinessRuleException("Usuário ou senha inválidos.");
        }

        var token = _tokenService.GenerateToken(user.Id, user.Email, user.Name);
        var expiresIn = 3600;

        _logger.LogInformation("Usuário autenticado com sucesso. Id: {UserId}, Email: {Email}", user.Id, user.Email);

        return new LoginResponseDto(token, "Bearer", expiresIn);
    }
}

