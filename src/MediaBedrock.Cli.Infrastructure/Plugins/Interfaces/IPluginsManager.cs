using Coderynx.Functional.Results;

namespace MediaBedrock.Cli.Infrastructure.Plugins.Interfaces;

public interface IPluginsManager : IAsyncDisposable
{
    void Initialize();
    Result<TComponent> ResolveComponent<TComponent>(string componentName) where TComponent : class;
}