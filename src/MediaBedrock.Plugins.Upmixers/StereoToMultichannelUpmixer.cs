using CSnakes.Runtime;
using CSnakes.Runtime.Python;
using MediaBedrock.Worker.Sdk.Processors;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MediaBedrock.Plugins.Upmixers;

[Processor("upmixers", "stereo_to_multichannel")]
public sealed class StereoToMultichannelUpmixer : IProcessor
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

        context.Logger.LogInformation("Running upmixer");

        try
        {
            var upmixer = environment.Upmix();
            upmixer.UpmixStereoToMultichannel(input, output, PyObject.From(new Dictionary<string, object>()));
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