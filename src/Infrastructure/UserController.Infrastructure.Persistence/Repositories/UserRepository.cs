using Itmo.Dev.Platform.Persistence.Abstractions.Commands;
using Itmo.Dev.Platform.Persistence.Abstractions.Connections;
using UserController.Application.Abstractions.Persistence.Repositories;
using UserController.Application.Models;

namespace UserController.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IPersistenceConnectionProvider _connectionProvider;

    public UserRepository(IPersistenceConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
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

    public Task<UserModel> GetUser(long userId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<UserModel> UpdateUser(UserModel user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}