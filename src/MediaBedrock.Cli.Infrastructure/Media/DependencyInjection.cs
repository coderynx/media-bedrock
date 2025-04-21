using MediaBedrock.Cli.Domain.Media;
using Microsoft.Extensions.DependencyInjection;

namespace MediaBedrock.Cli.Infrastructure.Media;

internal static class DependencyInjection
{
    internal static void AddMedia(this IServiceCollection services)
    {
        services.AddSingleton<IMediaInformationRetriever, MediaInfoRetriever>();
    }
}