using MediatR;
using Microsoft.Extensions.Logging;
using MeuRh.Application.DTOs;
using MeuRh.Domain.Interfaces.Service;

namespace MeuRh.Application.Queries.GetUsers;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, List<UserResponseDto>>
{
    private readonly IUserService _userService;
    private readonly ILogger<GetUsersQueryHandler> _logger;

    public GetUsersQueryHandler(
        IUserService userService,
        ILogger<GetUsersQueryHandler> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    public async Task<List<UserResponseDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            _logger.LogInformation("Buscando usuários ativos com filtro por nome: {Name}", request.Name);
        }
        else
        {
            _logger.LogInformation("Buscando todos os usuários ativos");
        }

        var users = await _userService.GetActiveUsersByNameAsync(request.Name, cancellationToken);

        _logger.LogInformation("Foram encontrados {Count} usuário(s) ativo(s)", users.Count);

        return users.Select(u => new UserResponseDto(u.Id, u.Name, u.Email, u.CreatedAt)).ToList();
    }
}

