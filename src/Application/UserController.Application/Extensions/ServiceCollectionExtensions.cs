using Microsoft.Extensions.DependencyInjection;
using UserController.Application.Contracts;

namespace UserController.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection collection)
    {
        // TODO: add services
        collection.AddScoped<IUserService, UserService>();
        return collection;
    }
}