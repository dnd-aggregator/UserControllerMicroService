using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using UserController.Application.Extensions;
using UserController.Infrastructure.Persistence.Background;
using UserController.Infrastructure.Persistence.Extensions;
using UserController.Presentation.Grpc.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets<Program>();

builder.Services.AddOptions<JsonSerializerSettings>();
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<JsonSerializerSettings>>().Value);

builder.Services.AddApplication();
builder.Services.AddInfrastructurePersistence();
builder.Services.AddPresentationGrpc();
builder.Services.AddHostedService<MigrationBackgroundService>();
builder.Services
    .AddControllers()
    .AddNewtonsoftJson();
builder.Services.AddSwaggerGen().AddEndpointsApiExplorer();

WebApplication app = builder.Build();

app.UseRouting();
app.UseSwagger();
app.UseSwaggerUI();

app.UsePresentationGrpc();
app.MapControllers();

await app.RunAsync();