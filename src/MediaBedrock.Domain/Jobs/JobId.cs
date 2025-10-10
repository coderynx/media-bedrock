namespace MediaBedrock.Domain.Jobs;

/// <summary>
///     Represents a unique identifier for a job.
/// </summary>
public sealed record JobId
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="JobId" /> class with the specified unique identifier.
    /// </summary>
    /// <param name="Value">The unique identifier value.</param>
    public JobId(Guid Value)
    {
        if (Value == Guid.Empty)
        {
            throw JobErrors.InvalidId();
        }

        this.Value = Value;
    }

    /// <summary>The unique identifier value.</summary>
    public Guid Value { get; }

    /// <summary>
    ///     Creates a new instance of the <see cref="JobId" /> class with a new unique identifier.
    /// </summary>
    /// <returns>A new <see cref="JobId" /> instance.</returns>
    public static JobId Create()
    {
        return new JobId(Guid.CreateVersion7());
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return Value.ToString();
    }
}