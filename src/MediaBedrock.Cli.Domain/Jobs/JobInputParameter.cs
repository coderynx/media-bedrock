using Coderynx.Functional.Result;

namespace MediaBedrock.Cli.Domain.Jobs;

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
            return JobErrors.InvalidInputString(keyValuePairs);
        }

        var inputs = parameters
            .Select(kvp => new JobInputParameter(kvp[0].Trim(), kvp[1].Trim()))
            .ToArray();

        return Result.Created(inputs);
    }
}

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
            return JobErrors.InvalidOutputString(keyValuePairs);
        }

        var outputs = parameters
            .Select(kvp => new JobOutputParameter(kvp[0].Trim(), kvp[1].Trim()))
            .ToArray();

        return Result.Created(outputs);
    }
}

public sealed record JobPropertyParameter(string Name, string Value)
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
            return JobErrors.InvalidPropertyString(keyValuePairs);
        }

        var properties = parameters
            .Select(kvp => new JobPropertyParameter(kvp[0].Trim(), kvp[1].Trim()))
            .ToArray();

        return Result.Created(properties);
    }
}