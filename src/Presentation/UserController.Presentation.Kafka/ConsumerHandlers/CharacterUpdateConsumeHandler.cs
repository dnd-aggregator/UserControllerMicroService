using Dnd;
using Itmo.Dev.Platform.Kafka.Consumer;
using UserController.Application.Contracts;
using UserController.Application.Models;

namespace UserController.Presentation.Kafka.ConsumerHandlers;

public class CharacterUpdateConsumeHandler : IKafkaConsumerHandler<CharacterUpdateKey, CharacterUpdateValue>
{
    private readonly ICharacterService _characterService;

    public CharacterUpdateConsumeHandler(ICharacterService characterService)
    {
        _characterService = characterService;
    }

    public async ValueTask HandleAsync(
        IEnumerable<IKafkaConsumerMessage<CharacterUpdateKey, CharacterUpdateValue>> messages,
        CancellationToken cancellationToken)
    {
        foreach (IKafkaConsumerMessage<CharacterUpdateKey, CharacterUpdateValue> message in messages)
        {
            if (message.Value.EventCase is CharacterUpdateValue.EventOneofCase.CharacterKill)
            {
                CharacterModel? character =
                    await _characterService.GetCharacter(message.Value.CharacterKill.CharacterId);
                if (character != null)
                {
                    character.Status = CharacterStatus.Dead;
                    await _characterService.UpdateCharacter(character);
                }
            }
            else if (message.Value.EventCase is CharacterUpdateValue.EventOneofCase.AddGear)
            {
                CharacterModel? character =
                    await _characterService.GetCharacter(message.Value.AddGear.CharacterId);
                if (character != null)
                {
                    var gear = character.Gear.ToList();
                    gear.Add(message.Value.AddGear.Gear);
                    character.Gear = gear;
                    await _characterService.UpdateCharacter(character);
                }
            }
            else if (message.Value.EventCase is CharacterUpdateValue.EventOneofCase.AddWeapon)
            {
                CharacterModel? character =
                    await _characterService.GetCharacter(message.Value.AddWeapon.CharacterId);
                if (character != null)
                {
                    var weapons = character.Weapons.ToList();
                    weapons.Add(message.Value.AddWeapon.Weapon);
                    character.Weapons = weapons;
                    await _characterService.UpdateCharacter(character);
                }
            }
        }
    }
}