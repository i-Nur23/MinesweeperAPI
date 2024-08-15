using Microsoft.EntityFrameworkCore;
using MinesweeperAPI.Models.Entities;

namespace MinesweeperAPI.Database
{
    public interface IApplicationDbContext : IDisposable
    {
        public DbSet<Game> Games { get; }

        public Task MigrateDatabaseAsync(CancellationToken cancellationToken);

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
