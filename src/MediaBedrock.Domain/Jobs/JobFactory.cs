using System.Text.RegularExpressions;
using Coderynx.Functional.Results;
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

        var job = Job.Create(template, createInputs.Value, createOutputs.Value);

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

    private static Result CreateSteps(
        Job job,
        JobTemplate template,
        JobPropertyParameter[] properties)
    {
        foreach (var step in template.Steps)
        {
            var stepProperties = new List<JobStepProperty>();
            foreach (var parameter in step.Properties)
            {
                var value = parameter.Value;
                foreach (Match match in EvaluateVariablesRegex().Matches(parameter.Value))
                {
                    var key = match.Groups[1].Value;

                    var property = properties.SingleOrDefault(p => p.Name.Equals(key));
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

            var createStepInputs = step.Inputs
                .Select(im => JobStepInput.Create(im.Name, new JobAssetName(im.Source)))
                .ToList();

            if (createStepInputs.Any(i => i.IsFailure))
            {
                return createStepInputs.First(i => i.IsFailure);
            }

            var stepInputs = createStepInputs
                .Select(i => i.Value)
                .ToList();

            var createStepOutputs = step.Outputs
                .Select(om => JobStepOutput.Create(om.Name, new JobAssetName(om.Destination)))
                .ToList();

            if (createStepOutputs.Any(o => o.IsFailure))
            {
                return createStepOutputs.First(o => o.IsFailure);
            }

            var stepOutputs = createStepOutputs
                .Select(o => o.Value)
                .ToList();

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
            
            job.CreateStep(
                name: createJobStepName.Value,
                order: createJobStepOrder.Value,
                processorName: step.ProcessorName,
                properties: stepProperties,
                inputs: stepInputs,
                outputs: stepOutputs);
        }

        return Result.Updated();
    }

    [GeneratedRegex(@"\$\{(\w+)\}")]
    private static partial Regex EvaluateVariablesRegex();
}