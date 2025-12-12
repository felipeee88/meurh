using FluentAssertions;
using MeuRh.Infra.Services;

namespace MeuRh.Tests.Services;

public class PasswordHasherTests
{
    private readonly PasswordHasher _passwordHasher;

    public PasswordHasherTests()
    {
        _passwordHasher = new PasswordHasher();
    }

    [Fact]
    public void HashPassword_ShouldReturnDifferentHashForSamePassword()
    {
        var password = "TestPassword123";
        var hash1 = _passwordHasher.HashPassword(password);
        var hash2 = _passwordHasher.HashPassword(password);

        hash1.Should().NotBeNullOrEmpty();
        hash2.Should().NotBeNullOrEmpty();
        hash1.Should().NotBe(hash2);
    }

    [Fact]
    public void VerifyPassword_WhenPasswordMatches_ShouldReturnTrue()
    {
        var password = "TestPassword123";
        var hash = _passwordHasher.HashPassword(password);

        var result = _passwordHasher.VerifyPassword(password, hash);

        result.Should().BeTrue();
    }

    [Fact]
    public void VerifyPassword_WhenPasswordDoesNotMatch_ShouldReturnFalse()
    {
        var password = "TestPassword123";
        var wrongPassword = "WrongPassword456";
        var hash = _passwordHasher.HashPassword(password);

        var result = _passwordHasher.VerifyPassword(wrongPassword, hash);

        result.Should().BeFalse();
    }

    [Fact]
    public void VerifyPassword_WhenHashIsInvalid_ShouldReturnFalse()
    {
        var password = "TestPassword123";
        var invalidHash = "invalid_hash_string";

        var result = _passwordHasher.VerifyPassword(password, invalidHash);

        result.Should().BeFalse();
    }

    [Fact]
    public void HashPassword_WithDifferentPasswords_ShouldReturnDifferentHashes()
    {
        var password1 = "Password1";
        var password2 = "Password2";

        var hash1 = _passwordHasher.HashPassword(password1);
        var hash2 = _passwordHasher.HashPassword(password2);

        hash1.Should().NotBe(hash2);
    }
}

