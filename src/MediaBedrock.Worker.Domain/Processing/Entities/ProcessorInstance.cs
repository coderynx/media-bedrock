using Coderynx.Functional.Results;
using Coderynx.Functional.Results.Successes;
using MediaBedrock.Core.Domain.Abstractions;
using MediaBedrock.Worker.Domain.Processing.DomainEvents;
using MediaBedrock.Worker.Domain.Processing.ValueObjects;

namespace MediaBedrock.Worker.Domain.Processing.Entities;

public sealed class ProcessorInstance : Entity
{
    private ProcessorInstance()
    {
    }

    private List<ProcessorInstanceInput> _inputs = [];
    private List<ProcessorInstanceOutput> _outputs = [];
    private List<ProcessorInstanceProperty> _properties = [];

    public ProcessorInstanceId Id { get; init; } = new();
    public required JobRunStepId JobRunStepId { get; init; }
    public required ProcessorName ProcessorName { get; init; }
    public ProcessorInstanceStatus Status { get; private set; } = ProcessorInstanceStatus.Pending;

    public IReadOnlyList<ProcessorInstanceInput> Inputs => _inputs.AsReadOnly();
    public IReadOnlyList<ProcessorInstanceOutput> Outputs => _outputs.AsReadOnly();
    public IReadOnlyList<ProcessorInstanceProperty> Properties => _properties.AsReadOnly();

    public static ProcessorInstance Create(
        JobRunStepId jobRunStepId,
        ProcessorName processorName,
        IEnumerable<ProcessorInstanceInput> inputs,
        IEnumerable<ProcessorInstanceOutput> outputs,
        IEnumerable<ProcessorInstanceProperty> properties)
    {
        return new ProcessorInstance
        {
            JobRunStepId = jobRunStepId,
            ProcessorName = processorName,
            _inputs = inputs.ToList(),
            _outputs = outputs.ToList(),
            _properties = properties.ToList()
        };
    }

    public Result Start()
    {
        if (Status is not ProcessorInstanceStatus.Pending)
        {
            return ProcessorInstanceErrors.InvalidStatusTransition(Status, ProcessorInstanceStatus.Running);
        }

        Status = ProcessorInstanceStatus.Running;

        var @event = new ProcessorInstanceStartedDomainEvent(Id);
        Raise(@event);

        return Success.Updated();
    }

    public void Complete()
    {
        if (Status is not ProcessorInstanceStatus.Running)
        {
            throw ProcessorInstanceErrors.InvalidStatusTransition(Status, ProcessorInstanceStatus.Completed);
        }

        Status = ProcessorInstanceStatus.Completed;

        var @event = new ProcessorInstanceCompletedDomainEvent(Id);
        Raise(@event);
    }

    public void Fail()
    {
        if (Status is not ProcessorInstanceStatus.Running)
        {
            throw ProcessorInstanceErrors.InvalidStatusTransition(Status, ProcessorInstanceStatus.Failed);
        }

        Status = ProcessorInstanceStatus.Failed;

        var @event = new ProcessorInstanceFailedDomainEvent(Id);
        Raise(@event);
    }
}