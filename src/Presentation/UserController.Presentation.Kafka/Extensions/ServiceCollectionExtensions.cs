using Dnd;
using Itmo.Dev.Platform.Kafka.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserController.Presentation.Kafka.ConsumerHandlers;

namespace UserController.Presentation.Kafka.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPresentationKafka(
        this IServiceCollection collection,
        IConfiguration configuration)
    {
        const string consumerKey = "Presentation:Kafka:Consumers";
        collection.AddPlatformKafka(builder => builder
            .ConfigureOptions(configuration.GetSection("Presentation:Kafka"))
            .AddConsumer(b => b
                .WithKey<CharacterUpdateKey>()
                .WithValue<CharacterUpdateValue>()
                .WithConfiguration(configuration.GetSection($"{consumerKey}:CharacterUpdate"))
                .DeserializeKeyWithProto()
                .DeserializeValueWithProto()
                .HandleWith<CharacterUpdateConsumeHandler>()));

        return collection;
    }
}