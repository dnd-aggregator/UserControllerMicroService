using UserController.Application.Abstractions.Repositories;
using UserController.Application.Contracts;
using UserController.Application.Contracts.Reqests;
using UserController.Application.Models;
using UserController.Application.Models.CharacterValidation;

namespace UserController.Application;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<long> RegisterUser(CreateUserModelRequest createUserModel, CancellationToken ct)
    {
        var model = new UserModel(createUserModel.Name, createUserModel.PhoneNumber);
        long id = await _userRepository.AddUser(model, ct);
        return id;
    }

    public async Task<UserModel?> GetUser(long userId, CancellationToken ct = default)
    {
        UserModel? model = await _userRepository.GetUser(userId, ct);
        return model;
    }

    public async Task UpdateUser(UserModel userModel, CancellationToken ct = default)
    {
        await _userRepository.UpdateUser(userModel, ct);
    }

    public async Task<CharacterValidationModel> ValidateUser(
        long userId,
        long characterId,
        CancellationToken ct = default)
    {
        UserModel? user = await GetUser(userId, ct);
        if (user == null) return new CharacterValidationModel.UserNotFoundValidationResult();
        if (user.Characters.FirstOrDefault(character => character.CharacterId == characterId) == null)
            return new CharacterValidationModel.CharacterNotFoundValidationResult();
        return new CharacterValidationModel.SuccessValidationResult();
    }
}