namespace MediaBedrock.Worker.Sdk.Processors;

public sealed record ProcessorProperty(string Name, string? Value)
{
    public string? GetValue()
    {
        return Value;
    }
}

public static class ProcessPropertyExtensions
{
    public static string GetValue(this ProcessorProperty? processorProperty, string defaultValue)
    {
        return processorProperty?.Value ?? defaultValue;
    }

    public static T? GetValue<T>(this ProcessorProperty? processorProperty, Func<string?, T> transform)
    {
        if (processorProperty?.Value is null)
        {
            return default;
        }

        try
        {
            return transform(processorProperty.Value);
        }
        catch (Exception)
        {
            return default;
        }
    }
}