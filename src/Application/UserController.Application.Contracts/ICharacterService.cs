using UserController.Application.Contracts.Reqests;
using UserController.Application.Models;

namespace UserController.Application.Contracts;

public interface ICharacterService
{
    Task<long> RegisterCharacter(
        CreateCharacterRequest createCharacterModel,
        long userId,
        CancellationToken ct = default);

    Task<CharacterModel?> GetCharacter(long characterId, CancellationToken ct = default);

    Task UpdateCharacter(CharacterModel characterModel, CancellationToken ct = default);
}