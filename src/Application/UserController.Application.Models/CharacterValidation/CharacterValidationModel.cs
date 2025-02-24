namespace UserController.Application.Models.CharacterValidation;

public record CharacterValidationModel()
{
    public record SuccessValidationResult : CharacterValidationModel;

    public record CharacterNotFoundValidationResult : CharacterValidationModel;

    public record UserNotFoundValidationResult : CharacterValidationModel;
}