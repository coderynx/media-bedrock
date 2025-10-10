using Microsoft.Extensions.Logging;

namespace MediaBedrock.Sdk.Processors;

public sealed record ProcessorContext
{
    private List<ProcessorInput> _inputs = [];
    private List<ProcessorOutput> _outputs = [];
    private List<ProcessorProperty> _properties = [];

    public required ILogger Logger { get; init; }
    public required string PluginPath { get; init; }

    public IReadOnlyList<ProcessorInput> Inputs => _inputs.AsReadOnly();
    public IReadOnlyList<ProcessorOutput> Outputs => _outputs.AsReadOnly();
    public IReadOnlyList<ProcessorProperty> Properties => _properties.AsReadOnly();

    public ProcessorInput GetInputRequired(string name)
    {
        return _inputs.FirstOrDefault(i => i.Name.Equals(name)) ??
               throw new ArgumentException($"Input {name} not found");
    }

    public ProcessorInput? GetInput(string name)
    {
        return _inputs.FirstOrDefault(i => i.Name.Equals(name));
    }

    public ProcessorOutput GetOutputRequired(string name)
    {
        return _outputs.FirstOrDefault(o => o.Name.Equals(name)) ??
               throw new ArgumentException($"Output {name} not found");
    }

    public ProcessorOutput? GetOutput(string name)
    {
        return _outputs.FirstOrDefault(o => o.Name.Equals(name));
    }

    public ProcessorProperty GetPropertyRequired(string name)
    {
        return _properties.FirstOrDefault(p => p.Name.Equals(name)) ??
               throw new ArgumentException($"Property {name} not found");
    }

    public ProcessorProperty? GetProperty(string name)
    {
        return _properties.FirstOrDefault(p => p.Name.Equals(name));
    }

    public static ProcessorContext Create(
        ILogger logger,
        string pluginPath,
        List<ProcessorInput> inputs,
        List<ProcessorOutput> outputs,
        List<ProcessorProperty> properties)
    {
        return new ProcessorContext
        {
            Logger = logger,
            PluginPath = pluginPath,
            _inputs = inputs,
            _outputs = outputs,
            _properties = properties
        };
    }
}