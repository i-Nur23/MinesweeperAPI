using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MinesweeperAPI.Database.BackgroundServices;
using MinesweeperAPI.Database.Repositories;
using MinesweeperAPI.Database.Repositories.Interfaces;

namespace MinesweeperAPI.Database
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDatabase(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("LocalConnection");
            services.AddDbContext<IApplicationDbContext, ApplicationDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            services.AddHostedService<DatabaseMigrationService>();

            services.AddScoped<IGamesRepository, GamesRepository>();

            return services;
        }
    }
}
