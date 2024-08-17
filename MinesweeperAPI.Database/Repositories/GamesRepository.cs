using Microsoft.EntityFrameworkCore;
using MinesweeperAPI.Database.Repositories.Interfaces;
using MinesweeperAPI.Models.Entities;
using System.Linq.Expressions;

namespace MinesweeperAPI.Database.Repositories
{
    internal class GamesRepository : IGamesRepository
    {
        private readonly IApplicationDbContext _dbContext;

        public GamesRepository(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Game game, CancellationToken cancellationToken)
        {
            await _dbContext.Games.AddAsync(game, cancellationToken);
        }

        public async Task<Game?> GetAsync(
            Guid gameId,
            CancellationToken cancellationToken)
        {
            return await _dbContext.Games.FirstOrDefaultAsync(game => game.Id.Equals(gameId));
        }

        public async Task<IEnumerable<Game>> GetAsync(
            Expression<Func<Game?, bool>> predicate, 
            CancellationToken cancellationToken)
        {
            return await _dbContext.Games.Where(predicate).ToListAsync(cancellationToken);
        }

        public async Task RemoveAsync(
            IEnumerable<Game> games, 
            CancellationToken cancellationToken)
        {
            _dbContext.Games.RemoveRange(games);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}