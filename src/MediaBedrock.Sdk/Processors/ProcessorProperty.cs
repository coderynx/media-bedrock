namespace MediaBedrock.Sdk.Processors;

public sealed record ProcessorProperty(string Name, string? Value)
{
    public string? GetValue()
    {
        return Value;
    }

    public string GetValue(string defaultValue)
    {
        return Value ?? defaultValue;
    }

    public T? Transform<T>(Func<string?, T> func)
    {
        if (Value is null)
        {
            return default;
        }

        try
        {
            return func(Value);
        }
        catch (Exception)
        {
            return default;
        }
    }
}