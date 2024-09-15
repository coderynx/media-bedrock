using Coderynx.Functional.Result;

namespace MediaBedrock.Cli.Domain.Jobs;

/// <summary>
/// Represents a unique identifier for a job.
/// </summary>
public sealed record JobId
{
    private JobId()
    {
    }

    /// <summary>
    /// Gets the value of the job identifier.
    /// </summary>
    public Guid Value { get; init; }

    /// <summary>
    /// Creates a new instance of the <see cref="JobId"/> class with a new unique identifier.
    /// </summary>
    /// <returns>A new <see cref="JobId"/> instance.</returns>
    public static JobId Create()
    {
        return new JobId
        {
            Value = Guid.CreateVersion7()
        };
    }

    /// <summary>
    /// Creates a new instance of the <see cref="JobId"/> class with the specified identifier.
    /// </summary>
    /// <param name="id">The identifier to use.</param>
    /// <returns>A <see cref="Result{T}"/> containing the <see cref="JobId"/> instance if the identifier is valid; otherwise, an error result indicating the identifier is invalid.</returns>
    public static Result<JobId> Create(Guid id)
    {
        if (id == Guid.Empty)
        {
            return JobErrors.InvalidId(id);
        }

        var jobId = new JobId
        {
            Value = id
        };
        return Result.Created(jobId);
    }

    /// <inheritdoc />
    public override string ToString() => Value.ToString();
}