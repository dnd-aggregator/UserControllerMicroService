using Microsoft.Extensions.DependencyInjection;
using UserController.Application.Contracts;

namespace UserController.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection collection)
    {
        collection.AddScoped<IUserService, UserService>();
        collection.AddScoped<ICharacterService, CharacterService>();
        return collection;
    }
}