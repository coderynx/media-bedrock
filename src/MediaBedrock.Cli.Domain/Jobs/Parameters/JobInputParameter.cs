using Coderynx.Functional.Results;

namespace MediaBedrock.Cli.Domain.Jobs.Parameters;

public sealed record JobInputParameter(string Name, string Uri)
{
    public static Result<JobInputParameter[]> CreateMultiple(string keyValuePairs)
    {
        var parameters = keyValuePairs
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(kvp => kvp.Split('=', StringSplitOptions.RemoveEmptyEntries))
            .ToArray();

        var isInvalid = parameters.Any(kvp =>
            kvp.Length != 2 ||
            string.IsNullOrWhiteSpace(kvp[0]) ||
            string.IsNullOrWhiteSpace(kvp[1]));

        if (isInvalid)
        {
            return JobParameterErrors.InvalidInputParameter(keyValuePairs);
        }

        var inputs = parameters
            .Select(kvp => new JobInputParameter(kvp[0].Trim(), kvp[1].Trim()))
            .ToArray();

        return Result.Created(inputs);
    }
}