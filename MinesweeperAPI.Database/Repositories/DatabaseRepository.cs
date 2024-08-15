using MinesweeperAPI.Database.Repositories.Interfaces;

namespace MinesweeperAPI.Database.Repositories
{
    public class DatabaseRepository : IDatabaseRepository
    {
        private readonly IApplicationDbContext _dbContext;

        public DatabaseRepository(
            IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
