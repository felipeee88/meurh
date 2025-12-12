using FluentAssertions;
using FluentValidation;
using MeuRh.Application.Commands.CreateUser;

namespace MeuRh.Tests.Validators;

public class CreateUserCommandValidatorTests
{
    private readonly CreateUserCommandValidator _validator;

    public CreateUserCommandValidatorTests()
    {
        _validator = new CreateUserCommandValidator();
    }

    [Fact]
    public async Task Validate_WhenNameIsEmpty_ShouldHaveValidationError()
    {
        var command = new CreateUserCommand("", "test@example.com", "Password123");
        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public async Task Validate_WhenNameIsTooShort_ShouldHaveValidationError()
    {
        var command = new CreateUserCommand("A", "test@example.com", "Password123");
        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public async Task Validate_WhenNameIsTooLong_ShouldHaveValidationError()
    {
        var longName = new string('A', 201);
        var command = new CreateUserCommand(longName, "test@example.com", "Password123");
        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public async Task Validate_WhenEmailIsEmpty_ShouldHaveValidationError()
    {
        var command = new CreateUserCommand("John Doe", "", "Password123");
        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Fact]
    public async Task Validate_WhenEmailIsInvalid_ShouldHaveValidationError()
    {
        var command = new CreateUserCommand("John Doe", "invalid-email", "Password123");
        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Fact]
    public async Task Validate_WhenEmailIsTooLong_ShouldHaveValidationError()
    {
        var longEmail = new string('a', 321);
        var command = new CreateUserCommand("John Doe", longEmail, "Password123");
        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Fact]
    public async Task Validate_WhenPasswordIsEmpty_ShouldHaveValidationError()
    {
        var command = new CreateUserCommand("John Doe", "test@example.com", "");
        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
    }

    [Fact]
    public async Task Validate_WhenPasswordIsTooShort_ShouldHaveValidationError()
    {
        var command = new CreateUserCommand("John Doe", "test@example.com", "12345");
        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
    }

    [Fact]
    public async Task Validate_WhenAllFieldsAreValid_ShouldNotHaveValidationErrors()
    {
        var command = new CreateUserCommand("John Doe", "test@example.com", "Password123");
        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeTrue();
    }
}

