using System.Text.RegularExpressions;
using Coderynx.Functional.Results;
using MediaBedrock.Cli.Domain.Jobs;
using MediaBedrock.Cli.Domain.Jobs.Batches;
using MediaBedrock.Cli.Domain.Jobs.Interfaces;
using MediaBedrock.Cli.Domain.Jobs.Parameters;
using MediaBedrock.Cli.Domain.Jobs.Steps;
using MediaBedrock.Cli.Domain.Jobs.Templates;
using MediaBedrock.Cli.Domain.Jobs.Templates.Interfaces;

namespace MediaBedrock.Cli.Application.Jobs;

/// <inheritdoc />
public sealed partial class JobFactory(IJobTemplatesRepository jobTemplatesRepository) : IJobFactory
{
    /// <inheritdoc />
    public async Task<Result<Job>> CreateAsync(JobParameters parameters)
    {
        var getTemplate = await jobTemplatesRepository.GetAsync(parameters.TemplateName);
        if (!getTemplate.IsSome)
        {
            return JobTemplateErrors.NotFound(parameters.TemplateName);
        }

        var template = getTemplate.ValueOrThrow();

        if (!template.Name.Equals(parameters.TemplateName))
        {
            return JobTemplateErrors.NotFound(template.Name);
        }

        var createInputs = CreateInputs(template, parameters.Inputs);
        if (createInputs.IsFailure)
        {
            return createInputs.Error;
        }

        var createOutputs = CreateOutputs(template, parameters.Outputs);
        if (createOutputs.IsFailure)
        {
            return createOutputs.Error;
        }

        var createSteps = CreateSteps(template, parameters.Properties);
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
    public async Task<Result<BatchJob>> CreateAsync(BatchJobParameters parameters)
    {
        var jobs = new List<Job>();
        foreach (var jobParameters in parameters.Entries)
        {
            var template = await jobTemplatesRepository.GetAsync(jobParameters.TemplateName);
            if (!template.IsSome)
            {
                return JobTemplateErrors.NotFound(jobParameters.TemplateName);
            }

            var createJob = await CreateAsync(jobParameters);
            if (createJob.IsFailure)
            {
                return createJob.Error;
            }

            jobs.Add(createJob.Value);
        }

        return BatchJob.Create(jobs);
    }

    private static Result<List<JobInput>> CreateInputs(JobTemplate template, JobInputParameter[] inputs)
    {
        var generatedInputs = new List<JobInput>();
        foreach (var i in inputs)
        {
            if (!template.Inputs.Any(tp => tp.Name.Equals(i.Name)))
            {
                return JobParameterErrors.InputParameterNotFound(i.Name);
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
                return JobParameterErrors.OutputParameterNotFound(o.Name);
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

                    var defaultValue = template.Properties.FirstOrDefault(p => p.Name.Equals(key));
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

            var createJobStepName = JobStepName.Create(s.Name);
            if (createJobStepName.IsFailure)
            {
                return createJobStepName.Error;
            }

            generatedSteps.Add(JobStep.Create(
                name: createJobStepName.Value,
                processorName: s.ProcessorName,
                properties: stepProperties,
                inputs: stepInputs,
                outputs: stepOutputs));
        }

        return Result.Created(generatedSteps);
    }

    [GeneratedRegex(@"\$\{(\w+)\}")]
    private static partial Regex EvaluateVariablesRegex();
}