using Coderynx.Functional.Result;
using MediaBedrock.Cli.Domain.Jobs;

namespace MediaBedrock.Cli.Application.Jobs.Interfaces;

public interface IJobSerializer
{
    Result<string> Serialize(Job job);
    Result<Job> Deserialize(string serialized);
}