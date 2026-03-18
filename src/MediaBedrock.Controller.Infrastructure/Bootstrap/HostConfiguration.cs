using Coderynx.MessagingKit;
using MediaBedrock.Controller.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MediaBedrock.Controller.Infrastructure.Bootstrap;

public static class HostConfiguration
{
    public static void UseControllerInfrastructure(this IServiceProvider services, IHostEnvironment environment)
    {
        services.UseMessaging();
        services.UseDatabase(environment);
    }

    private static void UseDatabase(this IServiceProvider services, IHostEnvironment environment)
    {
        if (environment.IsDevelopment())
        {
            using var scope = services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ControllerDbContext>();
            dbContext.Database.Migrate();
        }
    }

    // TODO: This should be removed and use the library method instead.
    private static void UseMessaging(this IServiceProvider services)
    {
        var busProvider = services.GetRequiredService<MessageBusManager>();

        busProvider.InitializeBuses();
        busProvider.WaitForBusesInitialization();
    }
}