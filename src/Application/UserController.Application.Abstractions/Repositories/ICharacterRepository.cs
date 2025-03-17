using UserController.Application.Models;

namespace UserController.Application.Abstractions.Repositories;

public interface ICharacterRepository
{
    Task<long> AddCharacter(CharacterModel character, CancellationToken cancellationToken = default);

    Task<CharacterModel?> GetCharacter(long characterId, CancellationToken cancellationToken = default);

    Task UpdateCharacter(CharacterModel character, CancellationToken cancellationToken = default);
}