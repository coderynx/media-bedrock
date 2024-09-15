namespace MediaBedrock.Sdk.Processors;

public sealed record ProcessorContext
{
    private IEnumerable<ProcessorInput> _inputs = [];
    private IEnumerable<ProcessorOutput> _outputs = [];
    private IEnumerable<ProcessorProperty> _properties = [];

    public IReadOnlyCollection<ProcessorOutput> Outputs => _outputs.ToArray();

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
        IEnumerable<ProcessorInput> inputs,
        IEnumerable<ProcessorOutput> outputs,
        IEnumerable<ProcessorProperty> properties)
    {
        return new ProcessorContext
        {
            _inputs = inputs,
            _outputs = outputs,
            _properties = properties
        };
    }
}