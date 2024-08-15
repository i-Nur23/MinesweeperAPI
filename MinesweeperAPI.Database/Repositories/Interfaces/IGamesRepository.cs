using MinesweeperAPI.Models.Entities;

namespace MinesweeperAPI.Database.Repositories.Interfaces
{
    public interface IGamesRepository
    {
        public Task<Game?> GetAsync(
            Guid gameId,
            CancellationToken cancellationToken);
    }
}
