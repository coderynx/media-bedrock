using MediaBedrock.Domain.BatchJobs;
using MediaBedrock.Domain.Jobs;
using MediaBedrock.Domain.Jobs.Parameters;
using MediaBedrock.Domain.JobTemplates;
using MediaBedrock.Domain.Processors;
using Shouldly;

namespace MediaBedrock.UnitTests.Jobs;

public sealed class JobFactoryTests
{
    private readonly JobFactory _jobFactory = new();

    [Fact]
    public void Create_ShouldReturnError_WhenTemplateNameDoesNotMatch()
    {
        // Arrange
        var template = JobTemplate.Create(
            name: new JobTemplateName("Template1"),
            version: new JobTemplateVersion(),
            author: new JobTemplateAuthor(),
            properties: [],
            inputs: [],
            outputs: []);

        var parameters = new JobParameters(new JobTemplateName("Template2"), [], [], []);

        // Act
        var result = _jobFactory.Create(template, parameters);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe(JobTemplateErrorCodes.NotFound);
    }

    [Fact]
    public void Create_ShouldReturnJob_WhenInputsOutputsAndStepsAreValid()
    {
        // Arrange
        var template = JobTemplate.Create(
            name: new JobTemplateName("Template1"),
            version: new JobTemplateVersion(),
            author: new JobTemplateAuthor(),
            inputs: [new JobTemplateInput("Input1")],
            outputs: [new JobTemplateOutput("Output1")],
            properties: []);

        var step = JobTemplateStep.Create(
            template: template,
            order: new JobTemplateStepOrder(1),
            name: new JobTemplateStepName("Step1"),
            processorName: ProcessorName.Create("namespace/processor").Value,
            inputs: [],
            outputs: [],
            properties: []);

        template.AddStepRange([step]);

        var parameters = new JobParameters(
            TemplateName: new JobTemplateName("Template1"),
            Inputs: [new JobInputParameter("Input1", "Uri1")],
            Outputs: [new JobOutputParameter("Output1", "Uri2")],
            Properties: []);

        // Act
        var result = _jobFactory.Create(template, parameters);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.TemplateId.ShouldBe(template.Id);
    }

    [Fact]
    public void CreateBatch_ShouldReturnError_WhenTemplateNotFound()
    {
        // Arrange
        var templates = new List<JobTemplate>();
        var parameters = new BatchJobParameters
        {
            Entries = [new JobParameters(new JobTemplateName("Template1"), [], [], [])]
        };

        // Act
        var result = _jobFactory.Create(templates, parameters);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe(JobTemplateErrorCodes.NotFound);
    }

    [Fact]
    public void CreateBatch_ShouldReturnBatchJob_WhenAllJobsAreValid()
    {
        // Arrange
        var template = JobTemplate.Create(
            name: new JobTemplateName("Template1"),
            version: new JobTemplateVersion(),
            author: new JobTemplateAuthor(),
            properties: [],
            inputs: [new JobTemplateInput("Input1")],
            outputs: [new JobTemplateOutput("Output1")]);

        var step = JobTemplateStep.Create(
            template: template,
            order: new JobTemplateStepOrder(1),
            name: new JobTemplateStepName("Step1"),
            processorName: ProcessorName.Create("namespace/processor").Value,
            inputs: [],
            outputs: [],
            properties: []);

        template.AddStepRange([step]);

        var templates = new List<JobTemplate> { template };
        var parameters = new BatchJobParameters
        {
            Entries =
            [
                new JobParameters(
                    TemplateName: new JobTemplateName("Template1"),
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
        result.Value.Jobs.First().TemplateId.ShouldBe(template.Id);
    }
}