using FluentAssertions;
using MeuRh.Application.DTOs;
using MeuRh.Application.Queries.GetUsers;
using MeuRh.Domain.Entities;
using MeuRh.Domain.Interfaces.Service;
using Microsoft.Extensions.Logging;
using Moq;

namespace MeuRh.Tests.Queries;

public class GetUsersQueryHandlerTests
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<ILogger<GetUsersQueryHandler>> _loggerMock;
    private readonly GetUsersQueryHandler _handler;

    public GetUsersQueryHandlerTests()
    {
        _userServiceMock = new Mock<IUserService>();
        _loggerMock = new Mock<ILogger<GetUsersQueryHandler>>();
        _handler = new GetUsersQueryHandler(_userServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WhenUsersExist_ShouldReturnListOfUserResponseDto()
    {
        var cancellationToken = CancellationToken.None;
        var users = new List<User>
        {
            new User("John Doe", "john@example.com", "hash1"),
            new User("Jane Smith", "jane@example.com", "hash2")
        };

        _userServiceMock
            .Setup(x => x.GetActiveUsersByNameAsync(null, cancellationToken))
            .ReturnsAsync(users);

        var query = new GetUsersQuery();
        var result = await _handler.Handle(query, cancellationToken);

        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result[0].Name.Should().Be("John Doe");
        result[0].Email.Should().Be("john@example.com");
        result[1].Name.Should().Be("Jane Smith");
        result[1].Email.Should().Be("jane@example.com");

        _userServiceMock.Verify(x => x.GetActiveUsersByNameAsync(null, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenNoUsersExist_ShouldReturnEmptyList()
    {
        var cancellationToken = CancellationToken.None;

        _userServiceMock
            .Setup(x => x.GetActiveUsersByNameAsync(null, cancellationToken))
            .ReturnsAsync(new List<User>());

        var query = new GetUsersQuery();
        var result = await _handler.Handle(query, cancellationToken);

        result.Should().NotBeNull();
        result.Should().BeEmpty();

        _userServiceMock.Verify(x => x.GetActiveUsersByNameAsync(null, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenNameFilterProvided_ShouldReturnFilteredUsers()
    {
        var cancellationToken = CancellationToken.None;
        var users = new List<User>
        {
            new User("John Doe", "john@example.com", "hash1")
        };

        _userServiceMock
            .Setup(x => x.GetActiveUsersByNameAsync("John", cancellationToken))
            .ReturnsAsync(users);

        var query = new GetUsersQuery("John");
        var result = await _handler.Handle(query, cancellationToken);

        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result[0].Name.Should().Be("John Doe");

        _userServiceMock.Verify(x => x.GetActiveUsersByNameAsync("John", cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenFilterIsCaseInsensitive_ShouldReturnFilteredUsers()
    {
        var cancellationToken = CancellationToken.None;
        var users = new List<User>
        {
            new User("John Doe", "john@example.com", "hash1")
        };

        _userServiceMock
            .Setup(x => x.GetActiveUsersByNameAsync("john", cancellationToken))
            .ReturnsAsync(users);

        var query = new GetUsersQuery("john");
        var result = await _handler.Handle(query, cancellationToken);

        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result[0].Name.Should().Be("John Doe");

        _userServiceMock.Verify(x => x.GetActiveUsersByNameAsync("john", cancellationToken), Times.Once);
    }
}

