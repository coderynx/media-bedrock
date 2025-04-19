using Coderynx.Functional.Results;

namespace MediaBedrock.Cli.Domain.Jobs.Templates;

public sealed record JobTemplateAuthor
{
    public static readonly JobTemplateAuthor Empty = new()
    {
        Value = string.Empty
    };

    private JobTemplateAuthor()
    {
    }

    public required string Value { get; init; }

    public static Result<JobTemplateAuthor> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return JobTemplateErrors.InvalidAuthor(value);
        }

        var author = new JobTemplateAuthor
        {
            Value = value
        };

        return Result.Created(author);
    }

    public override string ToString()
    {
        return Value;
    }
}