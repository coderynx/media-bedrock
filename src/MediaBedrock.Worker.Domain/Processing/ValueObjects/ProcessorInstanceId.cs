namespace MediaBedrock.Worker.Domain.Processing.ValueObjects;

public sealed record ProcessorInstanceId
{
    public ProcessorInstanceId(Guid value)
    {
        if (Value.Equals(Guid.Empty))
        {
            throw ProcessorInstanceErrors.InvalidId(value);
        }

        Value = value;
    }

    public ProcessorInstanceId()
    {
    }

    public Guid Value { get; init; } = Guid.NewGuid();
}