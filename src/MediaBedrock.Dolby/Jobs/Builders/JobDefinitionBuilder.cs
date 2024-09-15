using MediaBedrock.Dolby.Jobs.Models;

namespace MediaBedrock.Dolby.Jobs.Builders;

public interface IJobDefinitionBuilderInputStage
{
    IJobDefinitionBuilderFilterStage WithInput(IJobInput input);
}

public interface IJobDefinitionBuilderFilterStage
{
    IJobDefinitionBuilderOutputStage WithFilter(IJobFilter filter);
}

public interface IJobDefinitionBuilderOutputStage
{
    IJobDefinitionFinalStage WithOutput(IJobOutput output);
    IJobDefinitionFinalStage WithOutputs(params IJobOutput[] outputs);
}

public interface IJobDefinitionFinalStage
{
    JobDefinition Build();
}

public sealed class JobDefinitionBuilder :
    IJobDefinitionBuilderInputStage,
    IJobDefinitionBuilderFilterStage,
    IJobDefinitionBuilderOutputStage,
    IJobDefinitionFinalStage
{
    private IJobFilter _filter = null!;

    private IJobInput _input = null!;
    private List<IJobOutput> _outputs = [];

    internal JobDefinitionBuilder()
    {
    }

    public IJobDefinitionBuilderOutputStage WithFilter(IJobFilter filter)
    {
        _filter = filter;
        return this;
    }

    public IJobDefinitionBuilderFilterStage WithInput(IJobInput input)
    {
        _input = input;
        return this;
    }

    public IJobDefinitionFinalStage WithOutput(IJobOutput output)
    {
        _outputs.Add(output);
        return this;
    }

    public IJobDefinitionFinalStage WithOutputs(params IJobOutput[] outputs)
    {
        _outputs = outputs.ToList();
        return this;
    }

    public JobDefinition Build()
    {
        return new JobDefinition
        {
            Input = _input,
            Filter = _filter,
            Outputs = _outputs.ToArray()
        };
    }
}