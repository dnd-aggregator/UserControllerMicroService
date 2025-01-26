using Itmo.Dev.Platform.Persistence.Abstractions.Commands;
using Itmo.Dev.Platform.Persistence.Abstractions.Connections;
using System.Data.Common;
using UserController.Application.Abstractions.Persistence.Repositories;
using UserController.Application.Models;

namespace UserController.Infrastructure.Persistence.Repositories;

public class CharacterRepository : ICharacterRepository
{
    private readonly IPersistenceConnectionProvider _connectionProvider;

    public CharacterRepository(IPersistenceConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    public async Task<long> AddCharacter(CharacterModel character, CancellationToken cancellationToken)
    {
        const string sql = """
                           INSERT INTO characters (character_name, character_description, character_level, race, world_view,
                                                  speed, defence, health, max_health, strenth, dexterity, endurance, intelligence, wisdom, bonus,
                                                  personality_traits, ideals, bonds, flaws, history, user_id, status)
                           VALUES (@character_name, @character_description, @character_level, @race, @world_view,
                                   @speed, @defence, @health, @max_health, @strenth, @dexterity, @endurance, @intelligence, @wisdom, @bonus,
                                   @personality_traits, @ideals, @bonds, @flaws, @history, @user_id, @status)
                           RETURNING character_id;
                           """;

        await using IPersistenceConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using IPersistenceCommand command = connection.CreateCommand(sql)
            .AddParameter("@character_name", character.CharacterName)
            .AddParameter("@character_description", character.CharacterDescription)
            .AddParameter("@character_level", character.CharacterLevel)
            .AddParameter("@race", character.Race)
            .AddParameter("@world_view", character.WorldView)
            .AddParameter("@speed", character.Speed)
            .AddParameter("@defence", character.Defence)
            .AddParameter("@health", character.Health)
            .AddParameter("@max_health", character.MaxHealth)
            .AddParameter("@strenth", character.Strenth)
            .AddParameter("@dexterity", character.Dexterity)
            .AddParameter("@endurance", character.Endurance)
            .AddParameter("@intelligence", character.Intelligence)
            .AddParameter("@wisdom", character.Wisdom)
            .AddParameter("@bonus", character.Bonus)
            .AddParameter("@personality_traits", character.PersonalityTraits)
            .AddParameter("@ideals", character.Ideals)
            .AddParameter("@bonds", character.Bonds)
            .AddParameter("@flaws", character.Flaws)
            .AddParameter("@history", character.History)
            .AddParameter("@user_id", character.UserId)
            .AddParameter("@status", (int)character.Status);

        DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        long characterId = 0;

        while (await reader.ReadAsync(cancellationToken))
        {
            characterId = reader.GetInt64(0);
        }

        await reader.DisposeAsync();

        const string gearSql =
            "INSERT INTO character_gear (character_id, gear_item) VALUES (@character_id, @gear_item);";
        foreach (string gear in character.Gear)
        {
            await using IPersistenceCommand gearCommand = connection.CreateCommand(gearSql)
                .AddParameter("@character_id", characterId)
                .AddParameter("@gear_item", gear);
            await gearCommand.ExecuteNonQueryAsync(cancellationToken);
        }

        const string weaponsSql =
            "INSERT INTO character_weapons (character_id, weapon_item) VALUES (@character_id, @weapon_item);";
        foreach (string weapon in character.Weapons)
        {
            await using IPersistenceCommand weaponCommand = connection.CreateCommand(weaponsSql)
                .AddParameter("@character_id", characterId)
                .AddParameter("@weapon_item", weapon);
            await weaponCommand.ExecuteNonQueryAsync(cancellationToken);
        }

        const string activeSkillsSql =
            "INSERT INTO character_active_skills (character_id, active_skill) VALUES (@character_id, @active_skill);";
        foreach (string skill in character.ActiveSkills)
        {
            await using IPersistenceCommand skillCommand = connection.CreateCommand(activeSkillsSql)
                .AddParameter("@character_id", characterId)
                .AddParameter("@active_skill", skill);
            await skillCommand.ExecuteNonQueryAsync(cancellationToken);
        }

        const string passiveSkillsSql =
            "INSERT INTO character_passive_skills (character_id, passive_skill) VALUES (@character_id, @passive_skill);";
        foreach (string skill in character.PassiveSkills)
        {
            await using IPersistenceCommand skillCommand = connection.CreateCommand(passiveSkillsSql)
                .AddParameter("@character_id", characterId)
                .AddParameter("@passive_skill", skill);
            await skillCommand.ExecuteNonQueryAsync(cancellationToken);
        }

        return characterId;
    }

    public async Task<CharacterModel?> GetCharacter(long characterId, CancellationToken cancellationToken = default)
    {
        const string sql = """
                           SELECT character_name, character_description, character_level, race, world_view,
                                  speed, defence, health, max_health, strenth, dexterity, endurance, intelligence, wisdom, bonus,
                                  personality_traits, ideals, bonds, flaws, history, user_id, status
                           FROM characters
                           WHERE character_id = @character_id;
                           """;

        await using IPersistenceConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using IPersistenceCommand command = connection.CreateCommand(sql)
            .AddParameter("@character_id", characterId);

        await using DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        if (!await reader.ReadAsync(cancellationToken))
            return null;

        var character = new CharacterModel(
            reader.GetString(0),
            reader.GetString(1),
            reader.GetInt32(2),
            reader.GetString(3),
            reader.GetString(4),
            reader.GetInt32(5),
            reader.GetInt32(6),
            reader.GetInt32(7),
            reader.GetInt32(8),
            reader.GetInt32(9),
            reader.GetInt32(10),
            reader.GetInt32(11),
            reader.GetInt32(12),
            reader.GetInt32(13),
            reader.GetInt32(14),
            await GetCharacterRelatedData("character_gear", "gear_item", characterId, cancellationToken),
            await GetCharacterRelatedData("character_weapons", "weapon_item", characterId, cancellationToken),
            reader.GetString(15),
            reader.GetString(16),
            reader.GetString(17),
            reader.GetString(18),
            reader.GetString(19),
            await GetCharacterRelatedData("character_active_skills", "active_skill", characterId, cancellationToken),
            await GetCharacterRelatedData("character_passive_skills", "passive_skill", characterId, cancellationToken),
            reader.GetInt64(20));

        character.CharacterId = characterId;
        character.Status = (CharacterStatus)reader.GetInt32(21);

        return character;
    }

    public Task UpdateCharacter(CharacterModel character, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    private async Task<IReadOnlyCollection<string>> GetCharacterRelatedData(
        string tableName,
        string columnName,
        long characterId,
        CancellationToken cancellationToken)
    {
        string sql = $"SELECT {columnName} FROM {tableName} WHERE character_id = @character_id;";
        var items = new List<string>();
        await using IPersistenceConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);
        await using IPersistenceCommand command = connection.CreateCommand(sql)
            .AddParameter("@character_id", characterId);

        await using DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken)) items.Add(reader.GetString(0));
        return items;
    }
}