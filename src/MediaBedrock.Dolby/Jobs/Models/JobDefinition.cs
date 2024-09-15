using MediaBedrock.Dolby.Jobs.Builders;

namespace MediaBedrock.Dolby.Jobs.Models;

public interface IJobInput;

public interface IJobFilter;

public interface IJobOutput;

public sealed record JobDefinition
{
    public required IJobInput Input { get; init; }
    public required IJobFilter Filter { get; init; }
    public required IJobOutput[] Outputs { get; init; }

    public static JobDefinitionBuilder CreateBuilder()
    {
        return new JobDefinitionBuilder();
    }
}