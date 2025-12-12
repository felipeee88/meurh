using MeuRh.Domain.Entities;
using MeuRh.Domain.Exceptions;
using MeuRh.Domain.Interfaces.Repository;
using MeuRh.Domain.Interfaces.Service;

namespace MeuRh.Domain.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User> CreateUserAsync(string name, string email, string passwordHash, CancellationToken cancellationToken = default)
    {
        var emailExists = await _userRepository.EmailExistsAsync(email, cancellationToken);
        if (emailExists)
        {
            throw new BusinessRuleException("E-mail já cadastrado.");
        }

        var user = new User(name, email, passwordHash);
        await _userRepository.AddAsync(user, cancellationToken);

        return user;
    }

    public async Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _userRepository.GetByEmailAsync(email, cancellationToken);
    }

    public async Task<List<User>> GetActiveUsersByNameAsync(string? name, CancellationToken cancellationToken = default)
    {
        return await _userRepository.GetActiveUsersByNameAsync(name, cancellationToken);
    }

    public async Task<bool> DeactivateUserAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (user == null || !user.IsActive)
        {
            return false;
        }

        user.Deactivate();
        await _userRepository.UpdateAsync(user, cancellationToken);

        return true;
    }
}

