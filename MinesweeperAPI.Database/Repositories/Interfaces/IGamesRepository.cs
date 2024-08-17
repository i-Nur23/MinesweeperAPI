using MinesweeperAPI.Models.Entities;
using System.Linq.Expressions;

namespace MinesweeperAPI.Database.Repositories.Interfaces
{
    public interface IGamesRepository
    {
        public Task<Game?> GetAsync(
            Guid gameId,
            CancellationToken cancellationToken);

        public Task<IEnumerable<Game>> GetAsync(
            Expression<Func<Game?, bool>> predicate,
            CancellationToken cancellationToken);

        public Task RemoveAsync(
            IEnumerable<Game> games,
            CancellationToken token); 

        public Task AddAsync(
            Game game,
            CancellationToken cancellationToken);
    }
}
