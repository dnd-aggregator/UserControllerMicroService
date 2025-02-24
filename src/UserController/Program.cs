#pragma warning disable CA1506
using Itmo.Dev.Platform.Common.Extensions;
using Itmo.Dev.Platform.Events;
using Itmo.Dev.Platform.Observability;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using UserController.Application.Extensions;
using UserController.Infrastructure.Persistence.Extensions;
using UserController.Presentation.Grpc.Extensions;
using UserController.Presentation.Http.Extensions;
using UserController.Presentation.Kafka.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets<Program>();

builder.Services.AddOptions<JsonSerializerSettings>();
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<JsonSerializerSettings>>().Value);

builder.Services.AddPlatform();
builder.AddPlatformObservability();

builder.Services.AddApplication();
builder.Services.AddInfrastructurePersistence();
builder.Services.AddPresentationGrpc();
builder.Services.AddPresentationKafka(builder.Configuration);
builder.Services
    .AddControllers()
    .AddNewtonsoftJson()
    .AddPresentationHttp();

builder.Services.AddSwaggerGen().AddEndpointsApiExplorer();

builder.Services.AddPlatformEvents(b => b.AddPresentationKafkaHandlers());

builder.Services.AddUtcDateTimeProvider();

WebApplication app = builder.Build();

app.UseRouting();
app.UseSwagger();
app.UseSwaggerUI();

app.UsePlatformObservability();

app.UsePresentationGrpc();
app.MapControllers();

await app.RunAsync();