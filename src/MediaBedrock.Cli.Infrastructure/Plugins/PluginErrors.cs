using Coderynx.Functional;
using Coderynx.Functional.Results;

namespace MediaBedrock.Cli.Infrastructure.Plugins;

public static class PluginErrors
{
    public static readonly Error ContainerNotInitialized = new(
        ResultError: ResultError.Custom,
        Code: "Plugin.ContainerNotInitialized",
        Message: "The container is not initialized. Please initialize the container before using it."
    );
}