using FluentAssertions;
using MeuRh.Application.Commands.CreateUser;
using MeuRh.Application.DTOs;
using MeuRh.Application.Services;
using MeuRh.Domain.Entities;
using MeuRh.Domain.Exceptions;
using MeuRh.Domain.Interfaces.Service;
using Microsoft.Extensions.Logging;
using Moq;

namespace MeuRh.Tests.Commands.CreateUser;

public class CreateUserCommandHandlerTests
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<ILogger<CreateUserCommandHandler>> _loggerMock;
    private readonly CreateUserCommandHandler _handler;

    public CreateUserCommandHandlerTests()
    {
        _userServiceMock = new Mock<IUserService>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _loggerMock = new Mock<ILogger<CreateUserCommandHandler>>();
        _handler = new CreateUserCommandHandler(_userServiceMock.Object, _passwordHasherMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WhenEmailDoesNotExist_ShouldCreateUserAndReturnUserResponseDto()
    {
        var command = new CreateUserCommand("John Doe", "john@example.com", "Password123");
        var passwordHash = "hashed_password_123";
        var cancellationToken = CancellationToken.None;
        var user = new User("John Doe", "john@example.com", passwordHash);

        _passwordHasherMock
            .Setup(x => x.HashPassword(command.Password))
            .Returns(passwordHash);

        _userServiceMock
            .Setup(x => x.CreateUserAsync(command.Name, command.Email, passwordHash, cancellationToken))
            .ReturnsAsync(user);

        var result = await _handler.Handle(command, cancellationToken);

        result.Should().NotBeNull();
        result.Name.Should().Be(command.Name);
        result.Email.Should().Be(command.Email);
        result.Id.Should().NotBeEmpty();

        _passwordHasherMock.Verify(x => x.HashPassword(command.Password), Times.Once);
        _userServiceMock.Verify(x => x.CreateUserAsync(command.Name, command.Email, passwordHash, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenEmailAlreadyExists_ShouldThrowBusinessRuleException()
    {
        var command = new CreateUserCommand("John Doe", "john@example.com", "Password123");
        var passwordHash = "hashed_password_123";
        var cancellationToken = CancellationToken.None;

        _passwordHasherMock
            .Setup(x => x.HashPassword(command.Password))
            .Returns(passwordHash);

        _userServiceMock
            .Setup(x => x.CreateUserAsync(command.Name, command.Email, passwordHash, cancellationToken))
            .ThrowsAsync(new BusinessRuleException("E-mail já cadastrado."));

        var act = async () => await _handler.Handle(command, cancellationToken);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("E-mail já cadastrado.");

        _passwordHasherMock.Verify(x => x.HashPassword(command.Password), Times.Once);
        _userServiceMock.Verify(x => x.CreateUserAsync(command.Name, command.Email, passwordHash, cancellationToken), Times.Once);
    }
}

