using Npgsql;
using System.Data.Common;
using UserController.Application.Abstractions.Repositories;
using UserController.Application.Models;

namespace UserController.Infrastructure.Persistence.Repositories;

public class CharacterRepository : ICharacterRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public CharacterRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
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

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);

        var command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("@character_name", character.CharacterName),
                new NpgsqlParameter("@character_description", character.CharacterDescription),
                new NpgsqlParameter("@character_level", character.CharacterLevel),
                new NpgsqlParameter("@race", character.Race),
                new NpgsqlParameter("@world_view", character.WorldView),
                new NpgsqlParameter("@speed", character.Speed),
                new NpgsqlParameter("@defence", character.Defence),
                new NpgsqlParameter("@health", character.Health),
                new NpgsqlParameter("@max_health", character.MaxHealth),
                new NpgsqlParameter("@strenth", character.Strenth),
                new NpgsqlParameter("@dexterity", character.Dexterity),
                new NpgsqlParameter("@endurance", character.Endurance),
                new NpgsqlParameter("@intelligence", character.Intelligence),
                new NpgsqlParameter("@wisdom", character.Wisdom),
                new NpgsqlParameter("@bonus", character.Bonus),
                new NpgsqlParameter("@personality_traits", character.PersonalityTraits),
                new NpgsqlParameter("@ideals", character.Ideals),
                new NpgsqlParameter("@bonds", character.Bonds),
                new NpgsqlParameter("@flaws", character.Flaws),
                new NpgsqlParameter("@history", character.History),
                new NpgsqlParameter("@user_id", character.UserId),
                new NpgsqlParameter("@status", (int)character.Status),
            }
        };

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
            var gearCommand = new NpgsqlCommand(gearSql, connection)
            {
                Parameters =
                {
                    new NpgsqlParameter("@character_id", characterId),
                    new NpgsqlParameter("@gear_item", gear)
                }
            };
            await gearCommand.ExecuteNonQueryAsync(cancellationToken);
        }

        const string weaponsSql =
            "INSERT INTO character_weapons (character_id, weapon_item) VALUES (@character_id, @weapon_item);";
        foreach (string weapon in character.Weapons)
        {
            var weaponCommand = new NpgsqlCommand(weaponsSql, connection)
            {
                Parameters =
                {
                    new NpgsqlParameter("@character_id", characterId),
                    new NpgsqlParameter("@weapon_item", weapon)
                }
            };
            await weaponCommand.ExecuteNonQueryAsync(cancellationToken);
        }

        const string activeSkillsSql = """
                                       INSERT INTO character_active_skills (character_id, active_skill) 
                                       VALUES (@character_id, @active_skill)
                                       """;

        foreach (string skill in character.ActiveSkills)
        {
            var skillCommand = new NpgsqlCommand(activeSkillsSql, connection);
            skillCommand.Parameters.Add(new NpgsqlParameter("@character_id", characterId));
            skillCommand.Parameters.Add(new NpgsqlParameter("@active_skill", skill));
            await skillCommand.ExecuteNonQueryAsync(cancellationToken);
        }

        const string passiveSkillsSql =
            "INSERT INTO character_passive_skills (character_id, passive_skill) VALUES (@character_id, @passive_skill);";
        foreach (string skill in character.PassiveSkills)
        {
            var skillCommand = new NpgsqlCommand(passiveSkillsSql, connection)
            {
                Parameters =
                {
                    new NpgsqlParameter("@character_id", characterId),
                    new NpgsqlParameter("@passive_skill", skill)
                }
            };
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

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.Add(new NpgsqlParameter("@character_id", characterId));


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

    public async Task UpdateCharacter(CharacterModel character, CancellationToken cancellationToken = default)
    {
        const string updateCharacterSql = """
                                          UPDATE characters
                                          SET character_name = @character_name,
                                              character_description = @character_description,
                                              character_level = @character_level,
                                              race = @race,
                                              world_view = @world_view,
                                              speed = @speed,
                                              defence = @defence,
                                              health = @health,
                                              max_health = @max_health,
                                              strenth = @strenth,
                                              dexterity = @dexterity,
                                              endurance = @endurance,
                                              intelligence = @intelligence,
                                              wisdom = @wisdom,
                                              bonus = @bonus,
                                              personality_traits = @personality_traits,
                                              ideals = @ideals,
                                              bonds = @bonds,
                                              flaws = @flaws,
                                              history = @history,
                                              status = @status
                                          WHERE character_id = @character_id;
                                          """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);

        var characterCommand = new NpgsqlCommand(updateCharacterSql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("@character_id", character.CharacterId),
                new NpgsqlParameter("@character_name", character.CharacterName),
                new NpgsqlParameter("@character_description", character.CharacterDescription),
                new NpgsqlParameter("@character_level", character.CharacterLevel),
                new NpgsqlParameter("@race", character.Race),
                new NpgsqlParameter("@world_view", character.WorldView),
                new NpgsqlParameter("@speed", character.Speed),
                new NpgsqlParameter("@defence", character.Defence),
                new NpgsqlParameter("@health", character.Health),
                new NpgsqlParameter("@max_health", character.MaxHealth),
                new NpgsqlParameter("@strenth", character.Strenth),
                new NpgsqlParameter("@dexterity", character.Dexterity),
                new NpgsqlParameter("@endurance", character.Endurance),
                new NpgsqlParameter("@intelligence", character.Intelligence),
                new NpgsqlParameter("@wisdom", character.Wisdom),
                new NpgsqlParameter("@bonus", character.Bonus),
                new NpgsqlParameter("@personality_traits", character.PersonalityTraits),
                new NpgsqlParameter("@ideals", character.Ideals),
                new NpgsqlParameter("@bonds", character.Bonds),
                new NpgsqlParameter("@flaws", character.Flaws),
                new NpgsqlParameter("@history", character.History),
                new NpgsqlParameter("@status", (int)character.Status)
            }
        };

        await characterCommand.ExecuteNonQueryAsync(cancellationToken);

        await UpdateCharacterRelatedData(
            "character_gear",
            "gear_item",
            character.CharacterId,
            character.Gear,
            connection,
            cancellationToken);
        await UpdateCharacterRelatedData(
            "character_weapons",
            "weapon_item",
            character.CharacterId,
            character.Weapons,
            connection,
            cancellationToken);
        await UpdateCharacterRelatedData(
            "character_active_skills",
            "active_skill",
            character.CharacterId,
            character.ActiveSkills,
            connection,
            cancellationToken);
        await UpdateCharacterRelatedData(
            "character_passive_skills",
            "passive_skill",
            character.CharacterId,
            character.PassiveSkills,
            connection,
            cancellationToken);
    }

    private async Task UpdateCharacterRelatedData(
        string tableName,
        string columnName,
        long characterId,
        IReadOnlyCollection<string> newData,
        NpgsqlConnection connection,
        CancellationToken cancellationToken)
    {
        string deleteSql = $"""
                            DELETE FROM {tableName}
                            WHERE character_id = @character_id;
                            """;
        await using var deleteCommand = new NpgsqlCommand(deleteSql, connection);
        deleteCommand.Parameters.Add(new NpgsqlParameter("@character_id", characterId));
        await deleteCommand.ExecuteNonQueryAsync(cancellationToken);

        string insertSql = $"""
                            INSERT INTO {tableName} (character_id, {columnName}) 
                            VALUES (@character_id, @value)
                            """;
        foreach (string item in newData)
        {
            await using var insertCommand = new NpgsqlCommand(insertSql, connection);
            insertCommand.Parameters.Add(new NpgsqlParameter("@character_id", characterId));
            insertCommand.Parameters.Add(new NpgsqlParameter("@value", item));
            await insertCommand.ExecuteNonQueryAsync(cancellationToken);
        }
    }


    private async Task<IReadOnlyCollection<string>> GetCharacterRelatedData(
        string tableName,
        string columnName,
        long characterId,
        CancellationToken cancellationToken)
    {
        string sql = $"""
                      SELECT {columnName} 
                      FROM {tableName}
                      WHERE character_id = @character_id
                      """;

        var items = new List<string>();
        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.Add(new NpgsqlParameter("@character_id", characterId));

        await using DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken)) items.Add(reader.GetString(0));
        return items;
    }
}