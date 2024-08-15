using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MinesweeperAPI.Database.BackgroundServices
{
    public class DatabaseMigrationService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public DatabaseMigrationService(
            IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            using IServiceScope scope = _serviceProvider.CreateScope();
            using IApplicationDbContext dbContext =
                scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

            await dbContext.MigrateDatabaseAsync(cancellationToken);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }
    }
}
