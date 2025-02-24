using Microsoft.AspNetCore.Builder;
using UserController.Presentation.Grpc.Controllers;

namespace UserController.Presentation.Grpc.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UsePresentationGrpc(this IApplicationBuilder builder)
    {
        builder.UseEndpoints(routeBuilder =>
        {
            routeBuilder.MapGrpcService<UserGrpcController>();
            routeBuilder.MapGrpcService<CharacterGrpcController>();
            routeBuilder.MapGrpcReflectionService();
        });

        return builder;
    }
}