using MediatR;
using MeuRh.Application.DTOs;

namespace MeuRh.Application.Queries.GetUsers;

public record GetUsersQuery(string? Name = null) : IRequest<List<UserResponseDto>>;

