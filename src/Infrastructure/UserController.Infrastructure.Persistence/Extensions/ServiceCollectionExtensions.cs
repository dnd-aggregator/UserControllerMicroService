using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Npgsql;
using System.Data;
using UserController.Application.Abstractions.Repositories;
using UserController.Infrastructure.Persistence.Migrations;
using UserController.Infrastructure.Persistence.Repositories;

namespace UserController.Infrastructure.Persistence.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructurePersistence(this IServiceCollection collection)
    {
        collection.AddSingleton<NpgsqlDataSource>(provider =>
        {
            PostgresOptions options = provider.GetRequiredService<IOptionsMonitor<PostgresOptions>>().CurrentValue;
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(options.PostgresConnectionString());
            return dataSourceBuilder.Build();
        });

        collection.AddScoped<IDbConnection>(provider => new NpgsqlConnection(provider.GetRequiredService<IOptionsMonitor<PostgresOptions>>().CurrentValue.PostgresConnectionString()));

        collection
            .AddFluentMigratorCore()
            .ConfigureRunner(runner => runner
                .AddPostgres()
                .WithGlobalConnectionString(s =>
                {
                    IOptionsMonitor<PostgresOptions> cfg = s.GetRequiredService<IOptionsMonitor<PostgresOptions>>();
                    return cfg.CurrentValue.PostgresConnectionString();
                })
                .WithMigrationsIn(typeof(Initial).Assembly));

        collection.AddScoped<IUserRepository, UserRepository>();
        collection.AddScoped<ICharacterRepository, CharacterRepository>();

        return collection;
    }
}