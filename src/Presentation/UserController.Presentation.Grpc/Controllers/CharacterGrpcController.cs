using CharactersGrpc.Proto;
using Grpc.Core;
using UserController.Application.Contracts;
using UserController.Application.Contracts.Reqests;

namespace UserController.Presentation.Grpc.Controllers;

public class CharacterGrpcController : CharacterService.CharacterServiceBase
{
    private readonly ICharacterService _characterService;

    public CharacterGrpcController(ICharacterService characterService)
    {
        _characterService = characterService;
    }

    public override async Task<GetCharacterResponse> GetCharacter(
        GetCharacterRequest request,
        ServerCallContext context)
    {
        Application.Models.CharacterModel? character = await _characterService.GetCharacter(request.CharacterId);
        if (character == null) throw new RpcException(new Status(StatusCode.NotFound, "Character not found"));

        return new GetCharacterResponse
        {
            Character = new CharacterModel
            {
                CharacterId = character.CharacterId,
                UserId = character.UserId,
                CharacterName = character.CharacterName,
                CharacterDescription = character.CharacterDescription,
                CharacterLevel = character.CharacterLevel,
                Race = character.Race,
                WorldView = character.WorldView,
                Speed = character.Speed,
                Defence = character.Defence,
                Health = character.Health,
                MaxHealth = character.MaxHealth,
                Strength = character.Strenth,
                Dexterity = character.Dexterity,
                Endurance = character.Endurance,
                Intelligence = character.Intelligence,
                Wisdom = character.Wisdom,
                Bonus = character.Bonus,
                Gear = { character.Gear },
                Weapons = { character.Weapons },
                PersonalityTraits = character.PersonalityTraits,
                Ideals = character.Ideals,
                Bonds = character.Bonds,
                Flaws = character.Flaws,
                History = character.History,
                ActiveSkills = { character.ActiveSkills },
                PassiveSkills = { character.PassiveSkills },
                Status = character.Status.ToString(),
            },
        };
    }

    public override async Task<RegisterCharacterResponse> RegisterCharacter(
        RegisterCharacterRequest request,
        ServerCallContext context)
    {
        long characterId = await _characterService.RegisterCharacter(
            new CreateCharacterRequest(
                request.CharacterName,
                request.CharacterDescription,
                request.CharacterLevel,
                request.Race,
                request.WorldView,
                request.Speed,
                request.Defence,
                request.Health,
                request.MaxHealth,
                request.Strength,
                request.Dexterity,
                request.Endurance,
                request.Intelligence,
                request.Wisdom,
                request.Bonus,
                request.Gear.ToList(),
                request.Weapons.ToList(),
                request.PersonalityTraits,
                request.Ideals,
                request.Bonds,
                request.Flaws,
                request.History,
                request.ActiveSkills.ToList(),
                request.PassiveSkills.ToList()),
            request.UserId);
        return new RegisterCharacterResponse { CharacterId = characterId };
    }
}