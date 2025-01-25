using Itmo.Dev.Platform.Persistence.Abstractions.Extensions;
using Itmo.Dev.Platform.Persistence.Postgres.Extensions;
using Microsoft.Extensions.DependencyInjection;
using UserController.Application.Abstractions.Persistence;
using UserController.Application.Abstractions.Persistence.Repositories;
using UserController.Infrastructure.Persistence.Plugins;
using UserController.Infrastructure.Persistence.Repositories;

namespace UserController.Infrastructure.Persistence.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructurePersistence(this IServiceCollection collection)
    {
        collection.AddPlatformPersistence(persistence => persistence
            .UsePostgres(postgres => postgres
                .WithConnectionOptions(b => b.BindConfiguration("Infrastructure:Persistence:Postgres"))
                .WithMigrationsFrom(typeof(IAssemblyMarker).Assembly)
                .WithDataSourcePlugin<MappingPlugin>()));

        // TODO: add repositories
        collection.AddScoped<IPersistenceContext, PersistenceContext>();

        collection.AddScoped<IUserRepository, UserRepository>();
        collection.AddScoped<ICharacterRepository, CharacterRepository>();

        return collection;
    }
}