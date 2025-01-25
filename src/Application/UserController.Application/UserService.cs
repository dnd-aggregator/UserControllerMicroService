using UserController.Application.Abstractions.Persistence;
using UserController.Application.Contracts;
using UserController.Application.Contracts.Reqests;
using UserController.Application.Models;
using UserController.Application.Models.CharacterValidation;

namespace UserController.Application;

public class UserService : IUserService
{
    private readonly IPersistenceContext _userRepository;

    public UserService(IPersistenceContext userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<long> RegisterUser(CreateUserModelRequest createUserModel, CancellationToken ct)
    {
        var model = new UserModel(createUserModel.Name, createUserModel.PhoneNumber);
        long id = await _userRepository.UserRepository.AddUser(model, ct);
        return id;
    }

    public async Task<UserModel?> GetUser(long userId, CancellationToken ct = default)
    {
        UserModel? model = await _userRepository.UserRepository.GetUser(userId, ct);
        return model;
    }

    public async Task UpdateUser(UserModel userModel, CancellationToken ct = default)
    {
        await _userRepository.UserRepository.UpdateUser(userModel, ct);
    }

    public async Task<CharacterValidationModel> ValidateUser(
        long userId,
        long characterId,
        CancellationToken ct = default)
    {
        UserModel? user = await GetUser(userId, ct);
        if (user == null) return new CharacterValidationModel.UserNotFoundValidationResult();
        if (user.Characters.First(_ => _.CharacterId == characterId) == null)
            return new CharacterValidationModel.CharacterNotFoundValidationResult();
        return new CharacterValidationModel.SuccessValidationResult();
    }
}