using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MinesweeperAPI.Database.Repositories.Interfaces;
using MinesweeperAPI.Models.Entities;

namespace MinesweeperAPI.Database.BackgroundServices
{
    internal class GamesCleaningService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        private IServiceScope _serviceScope;

        public GamesCleaningService(
            IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested) 
            {
                try
                {
                    _serviceScope = _serviceProvider.CreateScope();
                    IGamesRepository gamesRepository = _serviceScope.ServiceProvider.GetRequiredService<IGamesRepository>();
                    DateTime now = DateTime.UtcNow;
                    DateTime borderForCompletedGames = now - TimeSpan.FromMinutes(5);
                    DateTime borderForUncompletedGames = now - TimeSpan.FromMinutes(30);

                    IEnumerable<Game> deletingGames = await gamesRepository.GetAsync(
                        game =>
                            game.IsCompleted && game.UpdatedAt < borderForCompletedGames ||
                            !game.IsCompleted && game.UpdatedAt < borderForUncompletedGames,
                        stoppingToken);

                    await gamesRepository.RemoveAsync(deletingGames, stoppingToken);
                }
                finally
                {
                    _serviceScope?.Dispose();
                    await Task.Delay(TimeSpan.FromMinutes(1));
                }
            }
        }
    }
}
