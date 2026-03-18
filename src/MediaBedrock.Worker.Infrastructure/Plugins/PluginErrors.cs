using Coderynx.Functional.Results.Errors;

namespace MediaBedrock.Worker.Infrastructure.Plugins;

public static class PluginErrorCodes
{
    public const string ContainerNotInitialized = "Plugin.ContainerNotInitialized";
    public const string ComponentNotFound = "Plugin.ComponentNotFound";
}

public static class PluginErrors
{
    public static readonly Error ContainerNotInitialized =
        Error.Custom(
            code: PluginErrorCodes.ContainerNotInitialized,
            message: "The container is not initialized. Please initialize the container before using it."
        );

    public static Error ComponentNotFound(string name)
    {
        return Error.NotFound(
            code: PluginErrorCodes.ComponentNotFound,
            message: $"The component with name '{name}' was not found in the container."
        );
    }
}