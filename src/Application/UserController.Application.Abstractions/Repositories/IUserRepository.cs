using UserController.Application.Models;

namespace UserController.Application.Abstractions.Repositories;

public interface IUserRepository
{
    Task<long> AddUser(UserModel user, CancellationToken cancellationToken = default);

    Task<UserModel?> GetUser(long userId, CancellationToken cancellationToken = default);

    Task UpdateUser(UserModel user, CancellationToken cancellationToken = default);
}