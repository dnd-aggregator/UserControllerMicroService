using UserController.Application.Contracts.Reqests;

namespace UserController.Application.Contracts;

public interface IUserService
{
    Task<long> RegisterUser(CreateUserModelRequest createUserModel, CancellationToken ct = default);
}