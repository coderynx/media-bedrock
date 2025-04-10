using Coderynx.Functional.Results;

namespace MediaBedrock.Cli.Domain.Jobs.Batches;

/// <summary>
///     Represents a unique identifier for a batch job.
/// </summary>
public sealed record BatchJobId
{
    private BatchJobId()
    {
    }

    /// <summary>
    ///     Gets the value of the batch job identifier.
    /// </summary>
    public Guid Value { get; init; }

    /// <summary>
    ///     Creates a new instance of the <see cref="BatchJobId" /> class with a new unique identifier.
    /// </summary>
    /// <returns>A new <see cref="BatchJobId" /> instance.</returns>
    public static BatchJobId Create()
    {
        return new BatchJobId
        {
            Value = Guid.CreateVersion7()
        };
    }

    /// <summary>
    ///     Creates a new instance of the <see cref="BatchJobId" /> class with the specified identifier.
    /// </summary>
    /// <param name="id">The identifier to use.</param>
    /// <returns>
    ///     A <see cref="Result" /> containing the <see cref="BatchJobId" /> instance if the identifier is valid; otherwise,
    ///     an error result indicating the identifier is invalid.
    /// </returns>
    public static Result<BatchJobId> Create(Guid id)
    {
        if (id == Guid.Empty)
        {
            return BatchJobErrors.InvalidId(id);
        }

        var jobId = new BatchJobId
        {
            Value = id
        };
        return Result.Created(jobId);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return Value.ToString();
    }
}