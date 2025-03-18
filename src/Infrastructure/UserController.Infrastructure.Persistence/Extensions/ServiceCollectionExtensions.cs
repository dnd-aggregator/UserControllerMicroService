using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using UserController.Application.Abstractions.Repositories;
using UserController.Infrastructure.Persistence.Repositories;

namespace UserController.Infrastructure.Persistence.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructurePersistence(this IServiceCollection collection)
    {
        var connectionString = "Host=localhost;Port=5431;Database=user_database;Username=postgres;Password=postgres;";
    
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
    
        var dataSource = dataSourceBuilder.Build();
        collection.AddSingleton(dataSource);
    
        collection
            .AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                .AddPostgres()
                .WithGlobalConnectionString(connectionString)
                .ScanIn(typeof(ServiceCollectionExtensions).Assembly).For.Migrations())
            .AddLogging(lb => lb.AddFluentMigratorConsole());

        collection.AddScoped<IUserRepository, UserRepository>();
        collection.AddScoped<ICharacterRepository, CharacterRepository>();

        return collection;
    }
}