using MediaBedrock.Domain.JobAssets.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace MediaBedrock.Infrastructure.Media;

internal static class DependencyInjection
{
    internal static void AddMedia(this IServiceCollection services)
    {
        services.AddSingleton<IMediaInformationRetriever, MediaInfoRetriever>();
    }
}