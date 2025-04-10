using Coderynx.Functional.Results;

namespace MediaBedrock.Cli.Domain.Jobs.Parameters;

public sealed record JobPropertyParameter(string Name, string? Value)
{
    public static Result<JobPropertyParameter[]> CreateMultiple(string keyValuePairs)
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
            return JobParameterErrors.InvalidPropertyParameter(keyValuePairs);
        }

        var properties = parameters
            .Select(kvp => new JobPropertyParameter(kvp[0].Trim(), kvp[1].Trim()))
            .ToArray();

        return Result.Created(properties);
    }
}