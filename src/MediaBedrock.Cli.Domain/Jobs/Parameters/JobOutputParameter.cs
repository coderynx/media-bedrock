using Coderynx.Functional.Results;

namespace MediaBedrock.Cli.Domain.Jobs.Parameters;

public sealed record JobOutputParameter(string Name, string Uri)
{
    public static Result<JobOutputParameter[]> CreateMultiple(string keyValuePairs)
    {
        var parameters = keyValuePairs
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(kvp => kvp.Split('=', StringSplitOptions.RemoveEmptyEntries))
            .ToArray();

        var isInvalid = parameters.Any(kvp =>
            kvp.Length is not 2 ||
            string.IsNullOrWhiteSpace(kvp[0]) ||
            string.IsNullOrWhiteSpace(kvp[1]));

        if (isInvalid)
        {
            return JobParameterErrors.InvalidOutputParameter(keyValuePairs);
        }

        var outputs = parameters
            .Select(kvp => new JobOutputParameter(kvp[0].Trim(), kvp[1].Trim()))
            .ToArray();

        return Result.Created(outputs);
    }
}