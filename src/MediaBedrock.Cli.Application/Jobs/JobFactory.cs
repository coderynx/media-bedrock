using System.Text.RegularExpressions;
using Coderynx.Functional.Result;
using MediaBedrock.Cli.Application.Jobs.Interfaces;
using MediaBedrock.Cli.Domain.Jobs;
using MediaBedrock.Cli.Domain.Jobs.Batch;
using MediaBedrock.Cli.Domain.Jobs.Processors;
using MediaBedrock.Cli.Domain.Jobs.Steps;
using MediaBedrock.Cli.Domain.Jobs.Templates;

namespace MediaBedrock.Cli.Application.Jobs;

/// <inheritdoc />
public sealed partial class JobFactory : IJobFactory
{
    /// <inheritdoc />
    public Result<Job> Create(
        JobTemplate template,
        JobInputParameter[] inputs,
        JobOutputParameter[] outputs,
        JobPropertyParameter[] properties)
    {
        var createInputs = CreateInputs(template, inputs);
        if (createInputs.IsFailure)
        {
            return createInputs.Error;
        }

        var createOutputs = CreateOutputs(template, outputs);
        if (createOutputs.IsFailure)
        {
            return createOutputs.Error;
        }

        var createSteps = CreateSteps(template, properties);
        if (createSteps.IsFailure)
        {
            return createSteps.Error;
        }

        var job = Job.Create(
            template.Name,
            createInputs.Value.ToArray(),
            createOutputs.Value.ToArray(),
            createSteps.Value.ToArray());

        return Result.Created(job);
    }

    /// <inheritdoc />
    public Result<IEnumerable<Job>> Create(JobTemplate template, BatchJob batchJob)
    {
        var jobs = new List<Job>();
        foreach (var entry in batchJob.Entries)
        {
            var createJob = Create(
                template: template,
                inputs: entry.Inputs.Select(i => i.ToJobInputParameter()).ToArray(),
                outputs: entry.Outputs.Select(o => o.ToJobOutputParameter()).ToArray(),
                properties: entry.Properties.Select(p => p.ToJobPropertyParameter()).ToArray());

            if (createJob.IsFailure)
            {
                return createJob.Error;
            }

            jobs.Add(createJob.Value);
        }

        return Result.Created(jobs.AsEnumerable());
    }

    private static Result<List<JobInput>> CreateInputs(JobTemplate template, JobInputParameter[] inputs)
    {
        var generatedInputs = new List<JobInput>();
        foreach (var i in inputs)
        {
            if (!template.Inputs.Any(tp => tp.Name.Equals(i.Name)))
            {
                return JobErrors.InputParameterNotFound(i.Name);
            }

            generatedInputs.Add(JobInput.Create(i.Name, i.Uri));
        }

        return Result.Created(generatedInputs);
    }

    private static Result<List<JobOutput>> CreateOutputs(JobTemplate template, JobOutputParameter[] outputs)
    {
        var generatedOutputs = new List<JobOutput>();
        foreach (var o in outputs)
        {
            if (!template.Outputs.Any(tp => tp.Name.Equals(o.Name)))
            {
                return JobErrors.OutputParameterNotFound(o.Name);
            }

            generatedOutputs.Add(JobOutput.Create(o.Name, o.Uri));
        }

        return Result.Created(generatedOutputs);
    }

    private static Result<List<JobStep>> CreateSteps(JobTemplate template, JobPropertyParameter[] properties)
    {
        var generatedSteps = new List<JobStep>();
        foreach (var s in template.Steps)
        {
            var stepProperties = new List<JobStepProperty>();
            foreach (var parameter in s.Properties)
            {
                var value = parameter.Value;
                foreach (Match match in EvaluateVariablesRegex().Matches(parameter.Value))
                {
                    var key = match.Groups[1].Value;

                    var property = properties.FirstOrDefault(p => p.Name.Equals(key));
                    if (property is not null)
                    {
                        value = value.Replace(match.Value, property.Value);
                        continue;
                    }

                    var defaultValue = template.Parameters.FirstOrDefault(p => p.Name.Equals(key));
                    if (defaultValue is null)
                    {
                        return JobErrors.PropertyNotFound(key);
                    }

                    value = value.Replace(match.Value, defaultValue.DefaultValue);
                }

                stepProperties.Add(JobStepProperty.Create(parameter.Name, value));
            }

            var stepInputs = s.Sinks.Select(im =>
            {
                // TODO: Checks if asset exists.

                return new JobStepSink(im.Name, im.Source);
            }).ToArray();

            var stepOutputs = s.Sources.Select(om =>
            {
                // TODO: Checks if asset exists.

                return new JobStepSource(om.Name, om.Destination);
            }).ToArray();

            var createProcessorName = ProcessorName.Create(s.ProcessorName);
            if (createProcessorName.IsFailure)
            {
                return createProcessorName.Error;
            }

            var createJobStepName = JobStepName.Create(s.Name);
            if (createJobStepName.IsFailure)
            {
                return createJobStepName.Error;
            }

            generatedSteps.Add(JobStep.Create(
                name: createJobStepName.Value,
                processorName: createProcessorName.Value,
                properties: stepProperties,
                inputs: stepInputs,
                outputs: stepOutputs));
        }

        return Result.Created(generatedSteps);
    }

    [GeneratedRegex(@"\$\{(\w+)\}")]
    private static partial Regex EvaluateVariablesRegex();
}