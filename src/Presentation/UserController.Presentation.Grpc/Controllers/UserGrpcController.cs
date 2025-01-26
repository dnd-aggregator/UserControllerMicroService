using Character.Validation;
using Grpc.Core;
using UserController.Application.Contracts;
using UserController.Application.Contracts.Reqests;

namespace UserController.Presentation.Grpc.Controllers;

public class UserGrpcController : UserGrpcService.UserGrpcServiceBase
{
    private readonly IUserService _userService;

    public UserGrpcController(IUserService userService)
    {
        _userService = userService;
    }

    public override async Task<CharacterValidationResponse> ValidateUser(
        ValidateUserRequest request,
        ServerCallContext context)
    {
        Application.Models.CharacterValidation.CharacterValidationModel validationResponse =
            await _userService.ValidateUser(request.UserId, request.CharacterId);
        var response = new CharacterValidationResponse();
        switch (validationResponse)
        {
            case Application.Models.CharacterValidation.CharacterValidationModel.SuccessValidationResult:
                response.Success = new SuccessValidationResult();
                break;
            case Application.Models.CharacterValidation.CharacterValidationModel.CharacterNotFoundValidationResult:
                response.CharacterNotFound = new CharacterNotFoundValidationResult();
                break;
            case Application.Models.CharacterValidation.CharacterValidationModel.UserNotFoundValidationResult:
                response.UserNotFound = new UserNotFoundValidationResult();
                break;
        }

        return response;
    }

    public override async Task<RegisterUserResponse> RegisterUser(CreateUserRequest request, ServerCallContext context)
    {
        long response = await _userService.RegisterUser(new CreateUserModelRequest(request.Name, request.PhoneNumber));
        return new RegisterUserResponse()
        {
            UserId = response,
        };
    }

    public override async Task<GetUserResponse> GetUser(GetUserRequest request, ServerCallContext context)
    {
        Application.Models.UserModel? response = await _userService.GetUser(request.UserId);
        if (response == null) throw new RpcException(new Status(StatusCode.NotFound, "User not found"));
        return new GetUserResponse()
        {
            User = new UserModel()
            {
                Id = response.Id,
                Name = response.Name,
                PhoneNumber = response.PhoneNumber,
            },
        };
    }
}