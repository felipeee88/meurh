using FluentAssertions;
using MeuRh.Domain.Entities;
using MeuRh.Domain.Exceptions;
using MeuRh.Domain.Interfaces.Repository;
using MeuRh.Domain.Services;
using Moq;

namespace MeuRh.Tests.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _userService = new UserService(_userRepositoryMock.Object);
    }

    [Fact]
    public async Task CreateUserAsync_WhenEmailDoesNotExist_ShouldCreateUser()
    {
        var name = "John Doe";
        var email = "john@example.com";
        var passwordHash = "hashed_password";
        var cancellationToken = CancellationToken.None;

        _userRepositoryMock
            .Setup(x => x.EmailExistsAsync(email, cancellationToken))
            .ReturnsAsync(false);

        _userRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<User>(), cancellationToken))
            .Returns(Task.CompletedTask);

        var result = await _userService.CreateUserAsync(name, email, passwordHash, cancellationToken);

        result.Should().NotBeNull();
        result.Name.Should().Be(name);
        result.Email.Should().Be(email);
        result.PasswordHash.Should().Be(passwordHash);
        result.IsActive.Should().BeTrue();

        _userRepositoryMock.Verify(x => x.EmailExistsAsync(email, cancellationToken), Times.Once);
        _userRepositoryMock.Verify(x => x.AddAsync(It.Is<User>(u => u.Name == name && u.Email == email), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task CreateUserAsync_WhenEmailExists_ShouldThrowBusinessRuleException()
    {
        var name = "John Doe";
        var email = "john@example.com";
        var passwordHash = "hashed_password";
        var cancellationToken = CancellationToken.None;

        _userRepositoryMock
            .Setup(x => x.EmailExistsAsync(email, cancellationToken))
            .ReturnsAsync(true);

        var act = async () => await _userService.CreateUserAsync(name, email, passwordHash, cancellationToken);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("E-mail já cadastrado.");

        _userRepositoryMock.Verify(x => x.EmailExistsAsync(email, cancellationToken), Times.Once);
        _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>(), cancellationToken), Times.Never);
    }

    [Fact]
    public async Task GetUserByEmailAsync_ShouldReturnUser()
    {
        var email = "john@example.com";
        var user = new User("John Doe", email, "hashed_password");
        var cancellationToken = CancellationToken.None;

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(email, cancellationToken))
            .ReturnsAsync(user);

        var result = await _userService.GetUserByEmailAsync(email, cancellationToken);

        result.Should().NotBeNull();
        result.Should().Be(user);

        _userRepositoryMock.Verify(x => x.GetByEmailAsync(email, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task GetActiveUsersByNameAsync_WhenNameProvided_ShouldReturnFilteredUsers()
    {
        var name = "John";
        var cancellationToken = CancellationToken.None;
        var users = new List<User>
        {
            new User("John Doe", "john@example.com", "hash1")
        };

        _userRepositoryMock
            .Setup(x => x.GetActiveUsersByNameAsync(name, cancellationToken))
            .ReturnsAsync(users);

        var result = await _userService.GetActiveUsersByNameAsync(name, cancellationToken);

        result.Should().NotBeNull();
        result.Should().HaveCount(1);

        _userRepositoryMock.Verify(x => x.GetActiveUsersByNameAsync(name, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task DeactivateUserAsync_WhenUserExistsAndIsActive_ShouldDeactivate()
    {
        var userId = Guid.NewGuid();
        var user = new User("John Doe", "john@example.com", "hashed_password");
        var cancellationToken = CancellationToken.None;

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId, cancellationToken))
            .ReturnsAsync(user);

        _userRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<User>(), cancellationToken))
            .Returns(Task.CompletedTask);

        var result = await _userService.DeactivateUserAsync(userId, cancellationToken);

        result.Should().BeTrue();
        user.IsActive.Should().BeFalse();

        _userRepositoryMock.Verify(x => x.GetByIdAsync(userId, cancellationToken), Times.Once);
        _userRepositoryMock.Verify(x => x.UpdateAsync(It.Is<User>(u => !u.IsActive), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task DeactivateUserAsync_WhenUserDoesNotExist_ShouldReturnFalse()
    {
        var userId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId, cancellationToken))
            .ReturnsAsync((User?)null);

        var result = await _userService.DeactivateUserAsync(userId, cancellationToken);

        result.Should().BeFalse();

        _userRepositoryMock.Verify(x => x.GetByIdAsync(userId, cancellationToken), Times.Once);
        _userRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<User>(), cancellationToken), Times.Never);
    }

    [Fact]
    public async Task DeactivateUserAsync_WhenUserIsAlreadyInactive_ShouldReturnFalse()
    {
        var userId = Guid.NewGuid();
        var user = new User("John Doe", "john@example.com", "hashed_password");
        user.Deactivate();
        var cancellationToken = CancellationToken.None;

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId, cancellationToken))
            .ReturnsAsync(user);

        var result = await _userService.DeactivateUserAsync(userId, cancellationToken);

        result.Should().BeFalse();

        _userRepositoryMock.Verify(x => x.GetByIdAsync(userId, cancellationToken), Times.Once);
        _userRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<User>(), cancellationToken), Times.Never);
    }
}

