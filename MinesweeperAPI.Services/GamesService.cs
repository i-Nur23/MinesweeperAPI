using MinesweeperAPI.Database;
using MinesweeperAPI.Database.Repositories.Interfaces;
using MinesweeperAPI.Models.Dtos;
using MinesweeperAPI.Services.Interfaces;

namespace MinesweeperAPI.Services
{
    internal class GamesService : IGamesService
    {
        private readonly IGamesRepository _gamesRepository;

        public GamesService(IGamesRepository gamesRepository)
        {
            _gamesRepository = gamesRepository;
        }

        public Task<GameDto> MakeTurnAsync(
            Guid gameId, 
            int row, 
            int col, 
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<GameDto> StartNewGameAsync(
            int width, 
            int height, 
            int minesCount, 
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
