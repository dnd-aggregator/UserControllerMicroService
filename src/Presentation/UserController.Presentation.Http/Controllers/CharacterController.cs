using Microsoft.AspNetCore.Mvc;
using UserController.Application.Contracts;
using UserController.Application.Contracts.Reqests;
using UserController.Application.Models;

namespace UserController.Presentation.Http.Controllers;

[ApiController]
[Route("api/v1/characters")]
public class CharacterController : ControllerBase
{
    private readonly ICharacterService _characterService;

    public CharacterController(ICharacterService characterService)
    {
        _characterService = characterService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateCharacter(
        [FromBody] CreateCharacterRequest characterModel,
        [FromQuery] long userId,
        CancellationToken cancellationToken)
    {
        long characterId = await _characterService.RegisterCharacter(characterModel, userId, cancellationToken);
        return new ObjectResult(characterId) { StatusCode = StatusCodes.Status201Created };
    }

    [HttpGet]
    public async Task<IActionResult> GetCharacterById(
        [FromQuery] long characerId,
        CancellationToken cancellationToken)
    {
        CharacterModel? model = await _characterService.GetCharacter(characerId, cancellationToken);
        if (model == null)
        {
            return NotFound();
        }

        return new ObjectResult(model) { StatusCode = StatusCodes.Status200OK };
    }

    [HttpPut]
    public async Task<IActionResult> UpdateCharacterById(
        [FromBody] CharacterModel characterModel,
        CancellationToken cancellationToken)
    {
        await _characterService.UpdateCharacter(characterModel, cancellationToken);
        return new OkResult();
    }
}