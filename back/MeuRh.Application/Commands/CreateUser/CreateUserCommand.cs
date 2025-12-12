using MediatR;
using MeuRh.Application.DTOs;

namespace MeuRh.Application.Commands.CreateUser;

public record CreateUserCommand(string Name, string Email, string Password) : IRequest<UserResponseDto>;

