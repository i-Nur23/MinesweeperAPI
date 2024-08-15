using MinesweeperAPI.Database;
using MinesweeperAPI.Database.Repositories.Interfaces;
using MinesweeperAPI.Models.Dtos;
using MinesweeperAPI.Models.Entities;
using MinesweeperAPI.Models.Exceptions.ResponseExceptions;
using MinesweeperAPI.Services.Interfaces;

namespace MinesweeperAPI.Services
{
    internal class GamesService : IGamesService
    {
        private readonly IGamesRepository _gamesRepository;
        private readonly IDatabaseRepository _databaseRepository;

        public GamesService(
            IGamesRepository gamesRepository, 
            IDatabaseRepository databaseRepository)
        {
            _gamesRepository = gamesRepository;
            _databaseRepository = databaseRepository;
        }

        public async Task<GameDto> StartNewGameAsync(
            int width,
            int height,
            int minesCount,
            CancellationToken cancellationToken)
        {
            if (width < 2 || width > 30)
            {
                throw new BadRequestException("Ширина поля должна быть не менее 2 и не более 30");
            }

            if (height < 2 || height > 30)
            {
                throw new BadRequestException("Высота поля должна быть не менее 2 и не более 30");
            }

            int maxMinesCount = height * width - 1;

            if (minesCount < 1 || minesCount > maxMinesCount)
            {
                throw new BadRequestException($"Количесто мин должно быть не менее 1 и не более {maxMinesCount}");
            }

            Guid gameId = Guid.NewGuid();

            Game game = new Game
            {
                Id = gameId,
                Height = height,
                Width = width,
                IsCompleted = false,
                MinesCount = minesCount,
            };

            await _gamesRepository.AddAsync(game, cancellationToken);
            await _databaseRepository.SaveChangesAsync(cancellationToken);

            return new GameDto 
            { 
                Height = height,
                Width = width,
                GameId = gameId,
                Completed = false,
                Field = game.Field,
                MinesCount = minesCount
            };
        }

        public Task<GameDto> MakeTurnAsync(
            Guid gameId, 
            int row, 
            int col, 
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
