using MediaBedrock.Application.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MediaBedrock.Infrastructure.Database.Bootstrap;

internal static class DependencyInjection
{
    public static void AddDatabase(this IServiceCollection services)
    {
        var databasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "database.db");

        services.AddDbContext<ApplicationDbContext>(options => { options.UseSqlite($"Data Source={databasePath}"); });
        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
    }
}