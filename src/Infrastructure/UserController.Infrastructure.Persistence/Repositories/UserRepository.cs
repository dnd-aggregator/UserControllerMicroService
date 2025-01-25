using Itmo.Dev.Platform.Persistence.Abstractions.Commands;
using Itmo.Dev.Platform.Persistence.Abstractions.Connections;
using System.Data.Common;
using UserController.Application.Abstractions.Persistence.Repositories;
using UserController.Application.Models;

namespace UserController.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IPersistenceConnectionProvider _connectionProvider;
    private readonly ICharacterRepository _characterRepository;

    public UserRepository(IPersistenceConnectionProvider connectionProvider, ICharacterRepository characterRepository)
    {
        _connectionProvider = connectionProvider;
        _characterRepository = characterRepository;
    }

    public async Task<long> AddUser(UserModel user, CancellationToken cancellationToken)
    {
        const string sql = """
                           INSERT INTO users (name, phone_number)
                           VALUES (@name, @phone_number)
                           RETURNING user_id";"
                           """;

        await using IPersistenceConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using IPersistenceCommand command = connection.CreateCommand(sql)
            .AddParameter("@name", user.Name)
            .AddParameter("@phone_number", user.PhoneNumber);

        int result = await command.ExecuteNonQueryAsync(cancellationToken);
        return result;
    }

    public async Task<UserModel?> GetUser(long userId, CancellationToken cancellationToken)
    {
        const string sql = """
                           SELECT user_id, name, phone_number
                           FROM users
                           WHERE user_id = @user_id;
                           """;

        await using IPersistenceConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using IPersistenceCommand command = connection.CreateCommand(sql)
            .AddParameter("@user_id", userId);

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

        await using IPersistenceConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using IPersistenceCommand command = connection.CreateCommand(sql)
            .AddParameter("@user_id", user.Id)
            .AddParameter("@name", user.Name)
            .AddParameter("@phone_number", user.PhoneNumber);

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

        await using IPersistenceConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using IPersistenceCommand command = connection.CreateCommand(sql)
            .AddParameter("@user_id", userId);

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