using UserController.Application.Contracts.Reqests;
using UserController.Application.Models;
using UserController.Application.Models.CharacterValidation;

namespace UserController.Application.Contracts;

public interface IUserService
{
    Task<long> RegisterUser(CreateUserModelRequest createUserModel, CancellationToken ct = default);

    Task<UserModel?> GetUser(long userId, CancellationToken ct = default);

    Task UpdateUser(UserModel userModel, CancellationToken ct = default);

    Task<CharacterValidationModel> ValidateUser(long userId, long characterId, CancellationToken ct = default);
}