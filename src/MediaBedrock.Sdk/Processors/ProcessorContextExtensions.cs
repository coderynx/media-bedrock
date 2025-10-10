namespace MediaBedrock.Sdk.Processors;

public static class ProcessorContextExtensions
{
    public static Dictionary<string, string?> ToDictionary(this IReadOnlyList<ProcessorProperty> properties)
    {
        return properties.ToDictionary(k => k.Name, v => v.GetValue());
    }
}