using FluentAssertions;
using MeuRh.Application.Commands.DeleteUser;
using MeuRh.Domain.Entities;
using MeuRh.Domain.Interfaces.Service;
using Microsoft.Extensions.Logging;
using Moq;

namespace MeuRh.Tests.Commands.DeleteUser;

public class DeleteUserCommandHandlerTests
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<ILogger<DeleteUserCommandHandler>> _loggerMock;
    private readonly DeleteUserCommandHandler _handler;

    public DeleteUserCommandHandlerTests()
    {
        _userServiceMock = new Mock<IUserService>();
        _loggerMock = new Mock<ILogger<DeleteUserCommandHandler>>();
        _handler = new DeleteUserCommandHandler(_userServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WhenUserExistsAndIsActive_ShouldDeactivateUser()
    {
        var command = new DeleteUserCommand(Guid.NewGuid());
        var cancellationToken = CancellationToken.None;

        _userServiceMock
            .Setup(x => x.DeactivateUserAsync(command.Id, cancellationToken))
            .ReturnsAsync(true);

        var result = await _handler.Handle(command, cancellationToken);

        result.Should().BeTrue();
        _userServiceMock.Verify(x => x.DeactivateUserAsync(command.Id, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotExist_ShouldReturnFalse()
    {
        var command = new DeleteUserCommand(Guid.NewGuid());
        var cancellationToken = CancellationToken.None;

        _userServiceMock
            .Setup(x => x.DeactivateUserAsync(command.Id, cancellationToken))
            .ReturnsAsync(false);

        var result = await _handler.Handle(command, cancellationToken);

        result.Should().BeFalse();
        _userServiceMock.Verify(x => x.DeactivateUserAsync(command.Id, cancellationToken), Times.Once);
    }
}

