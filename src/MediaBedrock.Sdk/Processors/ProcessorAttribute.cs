namespace MediaBedrock.Sdk.Processors;

[AttributeUsage(AttributeTargets.Class)]
public sealed class ProcessorAttribute(string @namespace, string name) : Attribute
{
    public string Namespace { get; } = @namespace.ToLower();
    public string Name { get; } = name.ToLower();
}