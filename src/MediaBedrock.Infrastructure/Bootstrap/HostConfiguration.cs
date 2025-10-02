using MediaBedrock.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MediaBedrock.Infrastructure.Bootstrap;

public static class HostConfiguration
{
    public static void UseInfrastructure(this IServiceProvider services, IHostEnvironment environment)
    {
        if (environment.IsDevelopment())
        {
            using var scope = services.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.Database.Migrate();
        }
    }
}