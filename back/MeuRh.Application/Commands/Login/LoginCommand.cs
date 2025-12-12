using MediatR;
using MeuRh.Application.DTOs;

namespace MeuRh.Application.Commands.Login;

public record LoginCommand(string Email, string Password) : IRequest<LoginResponseDto>;

