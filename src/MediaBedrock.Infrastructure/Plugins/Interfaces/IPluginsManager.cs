using Coderynx.Functional.Results;

namespace MediaBedrock.Infrastructure.Plugins.Interfaces;

public interface IPluginsManager : IAsyncDisposable
{
    void Initialize();
    Result<TComponent> ResolveComponent<TComponent>(string name) where TComponent : class;
}