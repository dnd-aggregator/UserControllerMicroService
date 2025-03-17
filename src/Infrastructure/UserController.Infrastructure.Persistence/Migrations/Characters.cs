using FluentMigrator;

namespace UserController.Infrastructure.Persistence.Migrations;

[Migration(1731950000, "create_characters_and_related_tables")]
public class Characters : Migration
{
    private const  string UpSql = """
        CREATE TABLE characters (
            character_id BIGSERIAL PRIMARY KEY,
            user_id BIGINT NOT NULL REFERENCES users(user_id) ON DELETE CASCADE,
            character_name VARCHAR(255) NOT NULL,
            character_description TEXT NOT NULL,
            character_level INT NOT NULL,
            race VARCHAR(100) NOT NULL,
            world_view VARCHAR(100) NOT NULL,
            speed INT NOT NULL,
            defence INT NOT NULL,
            health INT NOT NULL,
            max_health INT NOT NULL,
            strenth INT NOT NULL,
            dexterity INT NOT NULL,
            endurance INT NOT NULL,
            intelligence INT NOT NULL,
            wisdom INT NOT NULL,
            bonus INT NOT NULL,
            personality_traits TEXT NOT NULL,
            ideals TEXT NOT NULL,
            bonds TEXT NOT NULL,
            flaws TEXT NOT NULL,
            history TEXT NOT NULL,
            status INT NOT NULL
        );

        CREATE TABLE character_gear (
            id BIGSERIAL PRIMARY KEY,
            character_id BIGINT NOT NULL REFERENCES characters(character_id) ON DELETE CASCADE,
            gear_item VARCHAR(255) NOT NULL
        );

        CREATE TABLE character_weapons (
            id BIGSERIAL PRIMARY KEY,
            character_id BIGINT NOT NULL REFERENCES characters(character_id) ON DELETE CASCADE,
            weapon_item VARCHAR(255) NOT NULL
        );

        CREATE TABLE character_active_skills (
            id BIGSERIAL PRIMARY KEY,
            character_id BIGINT NOT NULL REFERENCES characters(character_id) ON DELETE CASCADE,
            active_skill VARCHAR(255) NOT NULL
        );

        CREATE TABLE character_passive_skills (
            id BIGSERIAL PRIMARY KEY,
            character_id BIGINT NOT NULL REFERENCES characters(character_id) ON DELETE CASCADE,
            passive_skill VARCHAR(255) NOT NULL
        );
        """;

    private const string GetDownSql = 
        """
        DROP TABLE character_passive_skills;
        DROP TABLE character_active_skills;
        DROP TABLE character_weapons;
        DROP TABLE character_gear;
        DROP TABLE characters;
        """;

    public override void Up()
    {
        Execute.Sql(UpSql);
    }

    public override void Down()
    {
        Execute.Sql(GetDownSql);
    }
}
