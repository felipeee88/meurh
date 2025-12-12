using MediatR;

namespace MeuRh.Application.Commands.DeleteUser;

public record DeleteUserCommand(Guid Id) : IRequest<bool>;

