using Microsoft.Extensions.DependencyInjection;
using MinesweeperAPI.Services.Interfaces;

namespace MinesweeperAPI.Services
{
    public static class DepndencyInjection
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IGamesService, GamesService>();

            return services;
        }
    }
}
