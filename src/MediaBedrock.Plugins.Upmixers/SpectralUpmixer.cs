using CSnakes.Runtime;
using CSnakes.Runtime.Python;
using MediaBedrock.Sdk.Processors;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MediaBedrock.Plugins.Upmixers;

[Processor("upmixers", "spectral_upmixer")]
public sealed class SpectralUpmixer : IProcessor
{
    public Task<ProcessorResult> ProcessAsync(ProcessorContext context, CancellationToken cancellationToken = default)
    {
        var scriptsPath = Path.Combine(context.PluginPath, "scripts");

        var builder = new ServiceCollection();
        builder.WithPython()
            .WithHome(scriptsPath)
            .WithVirtualEnvironment(Path.Join(scriptsPath, ".venv"))
            .FromRedistributable()
            .WithPipInstaller();

        var services = builder.BuildServiceProvider();

        var environment = services.GetRequiredService<IPythonEnvironment>();

        var input = context.GetInputRequired("input").GetAsFilePath();
        var output = context.GetOutputRequired("output").GetAsFilePath();

        context.Logger.LogInformation("Running spectral upmixer");

        try
        {
            var upmixer = environment.SpectralUpmix();
            upmixer.Upmix(input, output, PyObject.From(new Dictionary<string, object>()));
        }
        catch (Exception exception)
        {
            var result = ProcessorResult.Failure("Failed to execute upmixer", exception.InnerException);
            return Task.FromResult(result);
        }

        context.Logger.LogInformation("Upmixer completed");

        return Task.FromResult(ProcessorResult.Success());
    }
}