using FluentAssertions;
using MeuRh.Application.Commands.Login;

namespace MeuRh.Tests.Validators;

public class LoginCommandValidatorTests
{
    private readonly LoginCommandValidator _validator;

    public LoginCommandValidatorTests()
    {
        _validator = new LoginCommandValidator();
    }

    [Fact]
    public async Task Validate_WhenEmailIsEmpty_ShouldHaveValidationError()
    {
        var command = new LoginCommand("", "Password123");
        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Fact]
    public async Task Validate_WhenEmailIsInvalid_ShouldHaveValidationError()
    {
        var command = new LoginCommand("invalid-email", "Password123");
        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Fact]
    public async Task Validate_WhenPasswordIsEmpty_ShouldHaveValidationError()
    {
        var command = new LoginCommand("test@example.com", "");
        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
    }

    [Fact]
    public async Task Validate_WhenAllFieldsAreValid_ShouldNotHaveValidationErrors()
    {
        var command = new LoginCommand("test@example.com", "Password123");
        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeTrue();
    }
}

