using UserController.Application.Abstractions.Persistence;
using UserController.Application.Contracts;
using UserController.Application.Contracts.Reqests;
using UserController.Application.Models;

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
}