using System.Text.RegularExpressions;
using Coderynx.Functional.Results;
using Coderynx.Functional.Results.Successes;
using MediaBedrock.Domain.BatchJobs;
using MediaBedrock.Domain.JobAssets;
using MediaBedrock.Domain.Jobs.Interfaces;
using MediaBedrock.Domain.Jobs.Parameters;
using MediaBedrock.Domain.Jobs.Steps;
using MediaBedrock.Domain.JobTemplates;

namespace MediaBedrock.Domain.Jobs;

/// <inheritdoc />
public sealed partial class JobFactory : IJobFactory
{
    /// <inheritdoc />
    public Result<Job> Create(JobTemplate template, JobParameters parameters)
    {
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

        var job = Job.Create(template.Id, createInputs.Value, createOutputs.Value);

        var createSteps = CreateSteps(job, template, parameters.Properties);
        if (createSteps.IsFailure)
        {
            return createSteps.Error;
        }

        return Result.Created(job);
    }

    /// <inheritdoc />
    public Result<BatchJob> Create(List<JobTemplate> templates, BatchJobParameters parameters)
    {
        var jobs = new List<Job>();
        foreach (var jobParameters in parameters.Entries)
        {
            var template = templates.FirstOrDefault(t => t.Name.Equals(jobParameters.TemplateName));
            if (template is null)
            {
                return JobTemplateErrors.NotFound(jobParameters.TemplateName);
            }

            var createJob = Create(template, jobParameters);
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

    private static Result<List<JobOutput>> CreateOutputs(JobTemplate template, JobOutputParameter[] parameters)
    {
        var outputs = new List<JobOutput>();
        foreach (var o in parameters)
        {
            if (!template.Outputs.Any(tp => tp.Name.Equals(o.Name)))
            {
                return JobParameterErrors.OutputParameterNotFound(o.Name);
            }

            outputs.Add(JobOutput.Create(o.Name, o.Uri));
        }

        return Result.Created(outputs);
    }

    private static Result CreateSteps(Job job, JobTemplate template, JobPropertyParameter[] parameters)
    {
        foreach (var step in template.Steps)
        {
            var createJobStepName = JobStepName.Create(step.Name.Value);
            if (createJobStepName.IsFailure)
            {
                return createJobStepName.Error;
            }

            var createJobStepOrder = JobStepOrder.Create(step.Order.Value);
            if (createJobStepOrder.IsFailure)
            {
                return createJobStepOrder.Error;
            }

            var createStepProperties = CreateStepProperties(template, step, parameters);
            if (createStepProperties.IsFailure)
            {
                return createStepProperties.Error;
            }

            var stepInputs = CreateStepInputs(step);
            if (stepInputs.IsFailure)
            {
                return stepInputs.Error;
            }

            var stepOutputs = CreateStepOutputs(step);
            if (stepOutputs.IsFailure)
            {
                return stepOutputs.Error;
            }

            job.CreateStep(
                name: createJobStepName.Value,
                order: createJobStepOrder.Value,
                processorName: step.ProcessorName,
                properties: createStepProperties.Value,
                inputs: stepInputs.Value,
                outputs: stepOutputs.Value);
        }

        return Result.Updated();
    }

    private static Result<List<JobStepInput>> CreateStepInputs(JobTemplateStep step)
    {
        var createStepInputs = step.Inputs
            .Select(im => JobStepInput.Create(im.Name, new JobAssetName(im.Source)))
            .ToList();

        if (createStepInputs.Any(i => i.IsFailure))
        {
            return createStepInputs.First(i => i.IsFailure).Error;
        }

        return Success.Created(createStepInputs.Select(i => i.Value).ToList());
    }

    private static Result<List<JobStepOutput>> CreateStepOutputs(JobTemplateStep step)
    {
        var createStepOutputs = step.Outputs
            .Select(om => JobStepOutput.Create(om.Name, new JobAssetName(om.Destination)))
            .ToList();

        if (createStepOutputs.Any(o => o.IsFailure))
        {
            return createStepOutputs.First(o => o.IsFailure).Error;
        }

        return Success.Created(createStepOutputs.Select(o => o.Value).ToList());
    }

    private static Result<List<JobStepProperty>> CreateStepProperties(
        JobTemplate template,
        JobTemplateStep step,
        JobPropertyParameter[] parameters)
    {
        var stepProperties = new List<JobStepProperty>();
        foreach (var parameter in step.Properties)
        {
            var value = parameter.Value;
            foreach (Match match in EvaluateVariablesRegex().Matches(parameter.Value))
            {
                var key = match.Groups[1].Value;

                var property = parameters.SingleOrDefault(p => p.Name.Equals(key));
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

        return Success.Created(stepProperties);
    }

    [GeneratedRegex(@"\$\{(\w+)\}")]
    private static partial Regex EvaluateVariablesRegex();
}