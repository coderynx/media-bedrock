using MediaBedrock.Application.Jobs.Interfaces;
using MediaBedrock.Application.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace MediaBedrock.Infrastructure.Messaging.Bootstrap;

internal static class DependencyInjection
{
    public static void AddMessaging(this IServiceCollection services)
    {
        services.AddSingleton<InMemoryMessageQueue>();
        services.AddSingleton<IMessageBus, InMemoryMessageBus>();
        services.AddHostedService<MessageProcessorBackgroundService>();

        services.RegisterHandlerFromCurrentAssembly();
    }

    private static void RegisterHandlerFromCurrentAssembly(this IServiceCollection services)
    {
        var handlerType = typeof(IMessageConsumer<>);
        var assembly = handlerType.Assembly;
        var types = assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false } &&
                        t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerType));

        foreach (var type in types)
        {
            var interfaceType = type.GetInterfaces()
                .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerType);

            var messageType = interfaceType.GenericTypeArguments[0];
            var handlerInterface = handlerType.MakeGenericType(messageType);
            services.AddScoped(handlerInterface, type);
        }
    }
}