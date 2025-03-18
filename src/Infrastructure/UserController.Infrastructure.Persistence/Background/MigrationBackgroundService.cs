using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace UserController.Infrastructure.Persistence.Background;

public class MigrationBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public MigrationBackgroundService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await MigrationUp();
    }

    private async Task MigrationUp()
    {
        await using AsyncServiceScope scope = _scopeFactory.CreateAsyncScope();
        IMigrationRunner runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();
    }
}