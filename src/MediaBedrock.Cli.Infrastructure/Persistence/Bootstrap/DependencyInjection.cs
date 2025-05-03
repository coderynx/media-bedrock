using MediaBedrock.Cli.Application.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MediaBedrock.Cli.Infrastructure.Persistence.Bootstrap;

internal static class DependencyInjection
{
    public static void AddPersistence(this IServiceCollection services)
    {
        var databasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "persistence.db");

        services.AddDbContext<ApplicationDbContext>(options => { options.UseSqlite($"Data Source={databasePath}"); });
        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
    }
}