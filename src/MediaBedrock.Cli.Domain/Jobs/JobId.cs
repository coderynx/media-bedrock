namespace MediaBedrock.Cli.Domain.Jobs;

/// <summary>
///     Represents a unique identifier for a job.
/// </summary>
/// <param name="Value">The unique identifier value.</param>
public sealed record JobId(Guid Value)
{
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