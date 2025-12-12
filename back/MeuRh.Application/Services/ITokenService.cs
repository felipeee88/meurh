namespace MeuRh.Application.Services;

public interface ITokenService
{
    string GenerateToken(Guid userId, string email, string name);
}

