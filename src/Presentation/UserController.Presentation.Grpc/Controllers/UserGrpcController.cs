using Character.Validation;
using Grpc.Core;
using UserController.Application.Contracts;

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
}