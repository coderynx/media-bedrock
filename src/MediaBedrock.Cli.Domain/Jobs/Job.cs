using MediaBedrock.Cli.Domain.Jobs.Steps;
using MediaBedrock.Cli.Domain.JobTemplates;

namespace MediaBedrock.Cli.Domain.Jobs;

public sealed class Job
{
    private readonly List<JobStep> _steps = [];
    private List<JobInput> _inputs = [];
    private List<JobOutput> _outputs = [];

    private Job()
    {
    }

    public required JobId Id { get; init; }
    public required JobTemplate Template { get; init; }
    public IReadOnlyList<JobInput> Inputs => _inputs.AsReadOnly();
    public IReadOnlyList<JobOutput> Outputs => _outputs.AsReadOnly();
    public IReadOnlyList<JobStep> Steps => _steps.AsReadOnly();

    public static Job Create(
        JobTemplate template,
        IEnumerable<JobInput> inputs,
        IEnumerable<JobOutput> outputs)
    {
        return new Job
        {
            Id = JobId.Create(),
            Template = template,
            _inputs = inputs.ToList(),
            _outputs = outputs.ToList()
        };
    }

    public void AddStepRange(IEnumerable<JobStep> steps)
    {
        _steps.AddRange(steps);
    }
}