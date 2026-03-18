using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MediaBedrock.Worker.Infrastructure.Database;

internal static class HostConfiguration
{
    public static void UseDatabase(this IServiceProvider services, IHostEnvironment environment)
    {
        if (environment.IsDevelopment())
        {
            using var scope = services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<WorkerDbContext>();
            dbContext.Database.Migrate();
        }
    }
}