namespace MinesweeperAPI.Database.Repositories.Interfaces
{
    public interface IDatabaseRepository
    {
        public Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
