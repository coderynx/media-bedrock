using MediaBedrock.Worker.Application.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MediaBedrock.Worker.Infrastructure.Database;

internal static class DependencyInjection
{
    public static void AddDatabase(this IServiceCollection services)
    {
        var databasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "worker.db");

        services.AddDbContext<WorkerDbContext>(options => { options.UseSqlite($"Data Source={databasePath}"); });
        services.AddScoped<IWorkerDbContext>(provider => provider.GetRequiredService<WorkerDbContext>());
    }
}