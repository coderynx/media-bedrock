using MediaBedrock.Dolby.EncodingEngine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace MediaBedrock.Dolby.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static void AddDolbyEncodingEngine(this IServiceCollection services,
        Action<DolbyEncodingEngineSettings> settings)
    {
        services.AddOptions<DolbyEncodingEngineSettings>()
            .Configure(settings)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddEncodingEngine();
    }

    public static void AddDolbyMediaEncoder(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<DolbyEncodingEngineSettings>()
            .Bind(configuration.GetSection(DolbyEncodingEngineSettings.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.Configure<DolbyEncodingEngineSettings>(
            configuration.GetSection(DolbyEncodingEngineSettings.SectionName));

        services.AddEncodingEngine();
    }

    private static void AddEncodingEngine(this IServiceCollection services)
    {
        services.AddSingleton<IDolbyEncodingEngine, DolbyEncodingEngine>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<DolbyEncodingEngineSettings>>();

            return ActivatorUtilities.CreateInstance<DolbyEncodingEngine>(sp,
                options.Value.DirectoryPath,
                options.Value.UseWine);
        });
    }
}