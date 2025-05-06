using Coderynx.Functional.Results.Errors;

namespace MediaBedrock.Cli.Infrastructure.Plugins;

public static class PluginErrorCodes
{
    public const string ContainerNotInitialized = "Plugin.ContainerNotInitialized";
}

public static class PluginErrors
{
    public static readonly Error ContainerNotInitialized =
        Error.Custom(
            code: PluginErrorCodes.ContainerNotInitialized,
            message: "The container is not initialized. Please initialize the container before using it."
        );
}