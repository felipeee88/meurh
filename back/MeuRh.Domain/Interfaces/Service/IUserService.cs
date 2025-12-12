using MeuRh.Domain.Entities;

namespace MeuRh.Domain.Interfaces.Service;

public interface IUserService
{
    Task<User> CreateUserAsync(string name, string email, string passwordHash, CancellationToken cancellationToken = default);
    Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<List<User>> GetActiveUsersByNameAsync(string? name, CancellationToken cancellationToken = default);
    Task<bool> DeactivateUserAsync(Guid id, CancellationToken cancellationToken = default);
}

