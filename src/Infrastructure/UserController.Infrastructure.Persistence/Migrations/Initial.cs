using FluentMigrator;

namespace UserController.Infrastructure.Persistence.Migrations;

[Migration(1731949849, "initial")]
public class Initial : Migration
{
    private const string GetUpSql = """
        CREATE TABLE users (
            user_id BIGSERIAL PRIMARY KEY,
            name VARCHAR(255) NOT NULL,
            phone_number VARCHAR(20) NOT NULL
        );
        """;

    private const string GetDownSql = """
        drop table users;
        drop type order_state;
        """;

    public override void Up()
    {
        Execute.Sql(GetUpSql);
    }

    public override void Down()
    {
        Execute.Sql(GetDownSql);;
    }
}