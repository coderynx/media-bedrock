using MediaBedrock.Controller.Application.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MediaBedrock.Controller.Infrastructure.Database.Bootstrap;

internal static class DependencyInjection
{
    public static void AddDatabase(this IServiceCollection services)
    {
        var databasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "controller.db");

        services.AddDbContext<ControllerDbContext>(options => { options.UseSqlite($"Data Source={databasePath}"); });
        services.AddScoped<IControllerDbContext>(provider => provider.GetRequiredService<ControllerDbContext>());
    }
}