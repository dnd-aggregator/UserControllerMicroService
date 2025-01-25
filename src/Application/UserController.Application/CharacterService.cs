using UserController.Application.Abstractions.Persistence;
using UserController.Application.Contracts;
using UserController.Application.Contracts.Reqests;
using UserController.Application.Models;

namespace UserController.Application;

public class CharacterService : ICharacterService
{
    private readonly IPersistenceContext _context;

    public CharacterService(IPersistenceContext context)
    {
        _context = context;
    }

    public async Task<long> RegisterCharacter(
        CreateCharacterRequest createCharacterModel,
        long userId,
        CancellationToken ct = default)
    {
        UserModel? user = await _context.UserRepository.GetUser(userId, ct);
        if (user == null) throw new FileNotFoundException();

        var model = new CharacterModel(
            createCharacterModel.CharacterName,
            createCharacterModel.CharacterDescription,
            createCharacterModel.CharacterLevel,
            createCharacterModel.Race,
            createCharacterModel.WorldView,
            createCharacterModel.Speed,
            createCharacterModel.Defence,
            createCharacterModel.Health,
            createCharacterModel.MaxHealth,
            createCharacterModel.Strenth,
            createCharacterModel.Dexterity,
            createCharacterModel.Endurance,
            createCharacterModel.Intelligence,
            createCharacterModel.Wisdom,
            createCharacterModel.Bonus,
            createCharacterModel.Gear,
            createCharacterModel.Weapons,
            createCharacterModel.PersonalityTraits,
            createCharacterModel.Ideals,
            createCharacterModel.Bonds,
            createCharacterModel.Flaws,
            createCharacterModel.History,
            createCharacterModel.ActiveSkills,
            createCharacterModel.PassiveSkills,
            userId);
        long id = await _context.CharacterRepository.AddCharacter(model, ct);
        user.AddCharacter(model);
        await _context.UserRepository.UpdateUser(user, ct);
        return id;
    }

    public async Task<CharacterModel?> GetCharacter(long characterId, CancellationToken ct = default)
    {
        CharacterModel? model = await _context.CharacterRepository.GetCharacter(characterId, ct);
        return model;
    }

    public async Task UpdateCharacter(CharacterModel characterModel, CancellationToken ct = default)
    {
        await _context.CharacterRepository.UpdateCharacter(characterModel, ct);
    }
}