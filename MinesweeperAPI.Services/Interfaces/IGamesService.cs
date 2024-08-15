using MinesweeperAPI.Models.Dtos;

namespace MinesweeperAPI.Services.Interfaces
{
    public interface IGamesService
    {
        public Task<GameDto> StartNewGameAsync(
            int width,
            int height,
            int minesCount,
            CancellationToken cancellationToken);

        public Task<GameDto> MakeTurnAsync(
            Guid gameId,
            int row,
            int col,
            CancellationToken cancellationToken);
    }
}
