using Microsoft.EntityFrameworkCore;
using MinesweeperAPI.Models.Entities;

namespace MinesweeperAPI.Database
{
    internal class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public DbSet<Game> Games { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public async Task MigrateDatabaseAsync(CancellationToken cancellationToken)
        {
            await Database.MigrateAsync(cancellationToken);
        }
    }
}
