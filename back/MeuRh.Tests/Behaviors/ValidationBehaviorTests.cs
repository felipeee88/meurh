using FluentAssertions;
using FluentValidation;
using MediatR;
using MeuRh.Application.Behaviors;
using MeuRh.Application.Commands.CreateUser;
using MeuRh.Application.DTOs;
using Moq;

namespace MeuRh.Tests.Behaviors;

public class ValidationBehaviorTests
{
    [Fact]
    public async Task Handle_WhenNoValidatorsExist_ShouldCallNext()
    {
        var validators = new List<IValidator<CreateUserCommand>>();
        var behavior = new ValidationBehavior<CreateUserCommand, UserResponseDto>(validators);
        var request = new CreateUserCommand("John Doe", "test@example.com", "Password123");
        var expectedResponse = new UserResponseDto(Guid.NewGuid(), "John Doe", "test@example.com", DateTime.UtcNow);

        var nextCalled = false;
        RequestHandlerDelegate<UserResponseDto> next = () =>
        {
            nextCalled = true;
            return Task.FromResult(expectedResponse);
        };

        var result = await behavior.Handle(request, next, CancellationToken.None);

        nextCalled.Should().BeTrue();
        result.Should().Be(expectedResponse);
    }

    [Fact]
    public async Task Handle_WhenValidationPasses_ShouldCallNext()
    {
        var validator = new Mock<IValidator<CreateUserCommand>>();
        var validationResult = new FluentValidation.Results.ValidationResult();
        validator.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<CreateUserCommand>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        var validators = new List<IValidator<CreateUserCommand>> { validator.Object };
        var behavior = new ValidationBehavior<CreateUserCommand, UserResponseDto>(validators);
        var request = new CreateUserCommand("John Doe", "test@example.com", "Password123");
        var expectedResponse = new UserResponseDto(Guid.NewGuid(), "John Doe", "test@example.com", DateTime.UtcNow);

        var nextCalled = false;
        RequestHandlerDelegate<UserResponseDto> next = () =>
        {
            nextCalled = true;
            return Task.FromResult(expectedResponse);
        };

        var result = await behavior.Handle(request, next, CancellationToken.None);

        nextCalled.Should().BeTrue();
        result.Should().Be(expectedResponse);
    }

    [Fact]
    public async Task Handle_WhenValidationFails_ShouldThrowValidationException()
    {
        var validator = new Mock<IValidator<CreateUserCommand>>();
        var validationError = new FluentValidation.Results.ValidationFailure("Name", "Name is required.");
        var validationResult = new FluentValidation.Results.ValidationResult(new[] { validationError });
        
        validator.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<CreateUserCommand>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        var validators = new List<IValidator<CreateUserCommand>> { validator.Object };
        var behavior = new ValidationBehavior<CreateUserCommand, UserResponseDto>(validators);
        var request = new CreateUserCommand("", "test@example.com", "Password123");

        var nextCalled = false;
        RequestHandlerDelegate<UserResponseDto> next = () =>
        {
            nextCalled = true;
            return Task.FromResult(new UserResponseDto(Guid.NewGuid(), "", "", DateTime.UtcNow));
        };

        var act = async () => await behavior.Handle(request, next, CancellationToken.None);

        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
        nextCalled.Should().BeFalse();
    }
}

