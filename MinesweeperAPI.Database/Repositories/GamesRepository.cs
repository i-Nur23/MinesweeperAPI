using Microsoft.EntityFrameworkCore;
using MinesweeperAPI.Database.Repositories.Interfaces;
using MinesweeperAPI.Models.Entities;

namespace MinesweeperAPI.Database.Repositories
{
    internal class GamesRepository : IGamesRepository
    {
        private readonly IApplicationDbContext _dbContext;

        public GamesRepository(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Game?> GetAsync(
            Guid gameId,
            CancellationToken cancellationToken)
        {
            return await _dbContext.Games.FirstOrDefaultAsync(game => game.Id.Equals(gameId));
        }
    }
}
