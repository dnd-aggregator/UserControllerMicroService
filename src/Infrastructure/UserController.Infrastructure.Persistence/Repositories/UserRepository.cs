using Npgsql;
using System.Data.Common;
using UserController.Application.Abstractions.Repositories;
using UserController.Application.Models;

namespace UserController.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly NpgsqlDataSource _dataSource;
    private readonly ICharacterRepository _characterRepository;

    public UserRepository(NpgsqlDataSource dataSource, ICharacterRepository characterRepository)
    {
        _dataSource = dataSource;
        _characterRepository = characterRepository;
    }

    public async Task<long> AddUser(UserModel user, CancellationToken cancellationToken)
    {
        const string sql = """
                           INSERT INTO users (name, phone_number)
                           VALUES (@name, @phone_number)
                           RETURNING user_id";"
                           """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.Add(new NpgsqlParameter("name", user.Name));
        command.Parameters.Add(new NpgsqlParameter("phone_number", user.PhoneNumber));

        await using DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken)) return reader.GetInt64(0);
        throw new InvalidOperationException();
    }

    public async Task<UserModel?> GetUser(long userId, CancellationToken cancellationToken)
    {
        const string sql = """
                           SELECT user_id, name, phone_number
                           FROM users
                           WHERE user_id = @user_id;
                           """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);

        await using var command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("user_id", userId)
            }
        };

        await using DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
        if (await reader.ReadAsync(cancellationToken))
        {
            var user = new UserModel(reader.GetString(1), reader.GetString(2))
            {
                Id = reader.GetInt64(0),
            };

            foreach (CharacterModel characterModel in await GetCharactersForUser(user.Id, cancellationToken)) user.AddCharacter(characterModel);
            return user;
        }

        return null;
    }

    public async Task UpdateUser(UserModel user, CancellationToken cancellationToken)
    {
        const string sql = """
                           UPDATE users
                           SET name = @name,
                               phone_number = @phone_number
                           WHERE user_id = @user_id;
                           """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);

        await using var command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("user_id", user.Id),
                new NpgsqlParameter("name", user.Name),
                new NpgsqlParameter("phone_number", user.PhoneNumber)
            }
        };

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private async Task<List<CharacterModel>> GetCharactersForUser(long userId, CancellationToken cancellationToken)
    {
        var characters = new List<CharacterModel>();
        const string sql = """
                           SELECT character_id
                           FROM characters
                           WHERE user_id = @user_id;
                           """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.Add(new NpgsqlParameter("user_id", userId));

        await using DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            long characterId = reader.GetInt64(0);
            CharacterModel? character = await _characterRepository.GetCharacter(characterId, cancellationToken);
            if (character != null)
            {
                characters.Add(character);
            }
        }

        return characters;
    }
}