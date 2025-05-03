using System.Text.RegularExpressions;
using Coderynx.Functional.Results;
using MediaBedrock.Cli.Domain.BatchJobs;
using MediaBedrock.Cli.Domain.JobAssets;
using MediaBedrock.Cli.Domain.Jobs.Interfaces;
using MediaBedrock.Cli.Domain.Jobs.Parameters;
using MediaBedrock.Cli.Domain.Jobs.Steps;
using MediaBedrock.Cli.Domain.JobTemplates;

namespace MediaBedrock.Cli.Domain.Jobs;

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

        job.AddStepRange(createSteps.Value);

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

    private static Result<List<JobStep>> CreateSteps(
        Job job,
        JobTemplate template,
        JobPropertyParameter[] properties)
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

            var createStepInputs = s.Inputs.Select(im =>
            {
                var assetName = new JobAssetName(im.Source);
                return JobStepInput.Create(im.Name, assetName);
            }).ToList();

            if (createStepInputs.Any(i => i.IsFailure))
            {
                return createStepInputs.First(i => i.IsFailure).Error;
            }

            var createStepOutputs = s.Outputs.Select(om =>
            {
                var assetName = new JobAssetName(om.Destination);
                return JobStepOutput.Create(om.Name, assetName);
            }).ToList();

            if (createStepOutputs.Any(o => o.IsFailure))
            {
                return createStepOutputs.First(o => o.IsFailure).Error;
            }

            var createJobStepName = JobStepName.Create(s.Name.Value);
            if (createJobStepName.IsFailure)
            {
                return createJobStepName.Error;
            }

            var jobStep = JobStep.Create(
                job: job,
                name: createJobStepName.Value,
                processorName: s.ProcessorName,
                properties: stepProperties,
                inputs: createStepInputs.Select(i => i.Value).ToList(),
                outputs: createStepOutputs.Select(o => o.Value).ToList());

            generatedSteps.Add(jobStep);
        }

        return Result.Created(generatedSteps);
    }

    [GeneratedRegex(@"\$\{(\w+)\}")]
    private static partial Regex EvaluateVariablesRegex();
}