using FluentAssertions;
using MeuRh.Application.Commands.Login;
using MeuRh.Application.DTOs;
using MeuRh.Application.Services;
using MeuRh.Domain.Entities;
using MeuRh.Domain.Exceptions;
using MeuRh.Domain.Interfaces.Service;
using Microsoft.Extensions.Logging;
using Moq;

namespace MeuRh.Tests.Commands.Login;

public class LoginCommandHandlerTests
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly Mock<ILogger<LoginCommandHandler>> _loggerMock;
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _userServiceMock = new Mock<IUserService>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _tokenServiceMock = new Mock<ITokenService>();
        _loggerMock = new Mock<ILogger<LoginCommandHandler>>();
        _handler = new LoginCommandHandler(
            _userServiceMock.Object,
            _passwordHasherMock.Object,
            _tokenServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WhenUserExistsAndPasswordIsValid_ShouldReturnLoginResponseDto()
    {
        var command = new LoginCommand("john@example.com", "Password123");
        var cancellationToken = CancellationToken.None;
        var user = new User("John Doe", "john@example.com", "hashed_password");
        var accessToken = "jwt_token_123";
        var expiresIn = 3600;

        _userServiceMock
            .Setup(x => x.GetUserByEmailAsync(command.Email, cancellationToken))
            .ReturnsAsync(user);

        _passwordHasherMock
            .Setup(x => x.VerifyPassword(command.Password, user.PasswordHash))
            .Returns(true);

        _tokenServiceMock
            .Setup(x => x.GenerateToken(user.Id, user.Email, user.Name))
            .Returns(accessToken);

        var result = await _handler.Handle(command, cancellationToken);

        result.Should().NotBeNull();
        result.AccessToken.Should().Be(accessToken);
        result.TokenType.Should().Be("Bearer");
        result.ExpiresIn.Should().Be(expiresIn);

        _userServiceMock.Verify(x => x.GetUserByEmailAsync(command.Email, cancellationToken), Times.Once);
        _passwordHasherMock.Verify(x => x.VerifyPassword(command.Password, user.PasswordHash), Times.Once);
        _tokenServiceMock.Verify(x => x.GenerateToken(user.Id, user.Email, user.Name), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotExist_ShouldThrowBusinessRuleException()
    {
        var command = new LoginCommand("john@example.com", "Password123");
        var cancellationToken = CancellationToken.None;

        _userServiceMock
            .Setup(x => x.GetUserByEmailAsync(command.Email, cancellationToken))
            .ReturnsAsync((User?)null);

        var act = async () => await _handler.Handle(command, cancellationToken);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("Usuário ou senha inválidos.");

        _userServiceMock.Verify(x => x.GetUserByEmailAsync(command.Email, cancellationToken), Times.Once);
        _passwordHasherMock.Verify(x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _tokenServiceMock.Verify(x => x.GenerateToken(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenPasswordIsInvalid_ShouldThrowBusinessRuleException()
    {
        var command = new LoginCommand("john@example.com", "WrongPassword");
        var cancellationToken = CancellationToken.None;
        var user = new User("John Doe", "john@example.com", "hashed_password");

        _userServiceMock
            .Setup(x => x.GetUserByEmailAsync(command.Email, cancellationToken))
            .ReturnsAsync(user);

        _passwordHasherMock
            .Setup(x => x.VerifyPassword(command.Password, user.PasswordHash))
            .Returns(false);

        var act = async () => await _handler.Handle(command, cancellationToken);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("Usuário ou senha inválidos.");

        _userServiceMock.Verify(x => x.GetUserByEmailAsync(command.Email, cancellationToken), Times.Once);
        _passwordHasherMock.Verify(x => x.VerifyPassword(command.Password, user.PasswordHash), Times.Once);
        _tokenServiceMock.Verify(x => x.GenerateToken(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenUserIsInactive_ShouldThrowBusinessRuleException()
    {
        var command = new LoginCommand("john@example.com", "Password123");
        var cancellationToken = CancellationToken.None;
        var user = new User("John Doe", "john@example.com", "hashed_password");
        user.Deactivate();

        _userServiceMock
            .Setup(x => x.GetUserByEmailAsync(command.Email, cancellationToken))
            .ReturnsAsync((User?)null);

        var act = async () => await _handler.Handle(command, cancellationToken);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("Usuário ou senha inválidos.");

        _userServiceMock.Verify(x => x.GetUserByEmailAsync(command.Email, cancellationToken), Times.Once);
        _passwordHasherMock.Verify(x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _tokenServiceMock.Verify(x => x.GenerateToken(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }
}

