namespace MediaBedrock.Sdk.Processors;

public sealed record ProcessorProperty(string Name, object? Value)
{
    public T GetValue<T>()
    {
        if (Value is T value)
        {
            return value;
        }

        throw new InvalidOperationException($"Property {Name} is not of type {typeof(T).Name}");
    }

    public T GetValue<T>(T defaultValue)
    {
        if (Value is T value)
        {
            return value;
        }

        return defaultValue;
    }
}