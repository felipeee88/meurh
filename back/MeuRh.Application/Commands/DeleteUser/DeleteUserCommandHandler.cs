using MediatR;
using Microsoft.Extensions.Logging;
using MeuRh.Domain.Interfaces.Service;

namespace MeuRh.Application.Commands.DeleteUser;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, bool>
{
    private readonly IUserService _userService;
    private readonly ILogger<DeleteUserCommandHandler> _logger;

    public DeleteUserCommandHandler(
        IUserService userService,
        ILogger<DeleteUserCommandHandler> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando exclusão lógica do usuário. Id: {UserId}", request.Id);

        try
        {
            var result = await _userService.DeactivateUserAsync(request.Id, cancellationToken);

            if (result)
            {
                _logger.LogInformation("Usuário marcado como inativo com sucesso. Id: {UserId}", request.Id);
            }
            else
            {
                _logger.LogWarning("Tentativa de exclusão de usuário inexistente ou já inativo. Id: {UserId}", request.Id);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar usuário no banco de dados. Id: {UserId}", request.Id);
            throw;
        }
    }
}

