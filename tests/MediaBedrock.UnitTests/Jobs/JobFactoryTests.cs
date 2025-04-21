using MediaBedrock.Cli.Application.Jobs;
using MediaBedrock.Cli.Domain.Jobs.Batches;
using MediaBedrock.Cli.Domain.Jobs.Parameters;
using MediaBedrock.Cli.Domain.Jobs.Processors;
using MediaBedrock.Cli.Domain.JobTemplates;
using Shouldly;

namespace MediaBedrock.UnitTests.Jobs;

public sealed class JobFactoryTests
{
    private readonly JobFactory _jobFactory = new();

    [Fact]
    public void Create_ShouldReturnError_WhenTemplateNameDoesNotMatch()
    {
        // Arrange
        var template = new JobTemplate { Name = JobTemplateName.Create("Template1").Value };
        var parameters = new JobParameters(JobTemplateName.Create("Template2").Value, [], [], []);

        // Act
        var result = _jobFactory.Create(template, parameters);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe(JobTemplateErrors.NotFoundCode);
    }

    [Fact]
    public void Create_ShouldReturnJob_WhenInputsOutputsAndStepsAreValid()
    {
        // Arrange
        var template = new JobTemplate
        {
            Name = JobTemplateName.Create("Template1").Value,
            Inputs = [new JobTemplateInput { Name = "Input1" }],
            Outputs = [new JobTemplateOutput { Name = "Output1" }],
            Steps =
            [
                new JobTemplateStep
                {
                    Name = "Step1",
                    ProcessorName = ProcessorName.Create("namespace/processor").Value
                }
            ]
        };

        var parameters = new JobParameters(
            JobTemplateName.Create("Template1").Value,
            [new JobInputParameter("Input1", "Uri1")],
            [new JobOutputParameter("Output1", "Uri2")], []);

        // Act
        var result = _jobFactory.Create(template, parameters);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.TemplateName.ShouldBe(template.Name);
    }

    [Fact]
    public void CreateBatch_ShouldReturnError_WhenTemplateNotFound()
    {
        // Arrange
        var templates = new List<JobTemplate>();
        var parameters = new BatchJobParameters
        {
            Entries = [new JobParameters(JobTemplateName.Create("Template1").Value, [], [], [])]
        };

        // Act
        var result = _jobFactory.Create(templates, parameters);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe(JobTemplateErrors.NotFoundCode);
    }

    [Fact]
    public void CreateBatch_ShouldReturnBatchJob_WhenAllJobsAreValid()
    {
        // Arrange
        var template = new JobTemplate
        {
            Name = JobTemplateName.Create("Template1").Value,
            Inputs = [new JobTemplateInput { Name = "Input1" }],
            Outputs = [new JobTemplateOutput { Name = "Output1" }],
            Steps =
            [
                new JobTemplateStep
                {
                    Name = "Step1",
                    ProcessorName = ProcessorName.Create("namespace/processor").Value
                }
            ]
        };

        var templates = new List<JobTemplate> { template };
        var parameters = new BatchJobParameters
        {
            Entries =
            [
                new JobParameters(
                    TemplateName: JobTemplateName.Create("Template1").Value,
                    Inputs: [new JobInputParameter("Input1", "Uri1")],
                    Outputs: [new JobOutputParameter("Output1", "Uri2")],
                    Properties: [])
            ]
        };

        // Act
        var result = _jobFactory.Create(templates, parameters);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Jobs.Count().ShouldBe(1);
        result.Value.Jobs.First().TemplateName.ShouldBe(template.Name);
    }
}