using FluentMigrator;
using Itmo.Dev.Platform.Persistence.Postgres.Migrations;

namespace UserController.Infrastructure.Persistence.Migrations;

[Migration(1731949849, "initial")]
public class Initial : SqlMigration
{
    protected override string GetUpSql(IServiceProvider serviceProvider) =>
        """
        CREATE TABLE users (
            user_id BIGSERIAL PRIMARY KEY,
            name VARCHAR(255) NOT NULL,
            phone_number VARCHAR(20) NOT NULL
        );
        """;

    protected override string GetDownSql(IServiceProvider serviceProvider) =>
        """
        drop table users;
        drop type order_state;
        """;
}