using AutoFixture;
using Coderynx.Functional.Options;
using MediaBedrock.Cli.Application.Jobs;
using MediaBedrock.Cli.Domain.Jobs.Batches;
using MediaBedrock.Cli.Domain.Jobs.Interfaces;
using MediaBedrock.Cli.Domain.Jobs.Parameters;
using MediaBedrock.Cli.Domain.Jobs.Processors;
using MediaBedrock.Cli.Domain.Jobs.Templates;
using NSubstitute;
using Shouldly;

namespace MediaBedrock.UnitTests.Jobs;

public sealed class JobFactoryTests
{
    private readonly IFixture _fixture = new Fixture();
    private readonly JobFactory _jobFactory;
    private readonly IJobTemplatesRepository _jobTemplatesRepository;

    public JobFactoryTests()
    {
        _jobTemplatesRepository = Substitute.For<IJobTemplatesRepository>();

        var jobTemplate = _fixture.Build<JobTemplate>()
            .With(x => x.Name, JobTemplateName.Create("TestTemplate").Value)
            .Create();

        _jobTemplatesRepository.GetAsync(Arg.Is<JobTemplateName>(name => name.Value.Equals("TestTemplate")))
            .Returns(Option<JobTemplate>.Some(jobTemplate));

        _jobTemplatesRepository.GetAsync(Arg.Any<JobTemplateName>())
            .Returns(Option<JobTemplate>.None());

        _jobFactory = new JobFactory(_jobTemplatesRepository);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnJob_WhenTemplateExists()
    {
        // Arrange
        var templateName = JobTemplateName.Create("TestTemplate").Value;
        var jobTemplate = new JobTemplate
        {
            Name = templateName,
            Version = "1.0",
            Inputs = [new JobTemplateInput { Name = "Input1" }],
            Outputs = [new JobTemplateOutput { Name = "Output1" }],
            Steps =
            [
                new JobTemplateStep { Name = "Step1", ProcessorName = new ProcessorName("namespace", "processor") }
            ]
        };

        var jobParameters = new JobParameters(
            templateName,
            [new JobInputParameter("Input1", "input-uri")],
            [new JobOutputParameter("Output1", "output-uri")],
            [new JobPropertyParameter("Property1", "Value1")]);

        _jobTemplatesRepository.GetAsync(templateName).Returns(Option<JobTemplate>.Some(jobTemplate));

        // Act
        var result = await _jobFactory.CreateAsync(jobParameters);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.TemplateName.ShouldBe(templateName);
        result.Value.Inputs.ShouldHaveSingleItem();
        result.Value.Outputs.ShouldHaveSingleItem();
        result.Value.Steps.ShouldHaveSingleItem();
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnError_WhenTemplateDoesNotExist()
    {
        // Arrange
        var templateName = JobTemplateName.Create("NonExistentTemplate").Value;
        var jobParameters = new JobParameters(
            templateName,
            [],
            [],
            []);

        _jobTemplatesRepository.GetAsync(templateName).Returns(Option<JobTemplate>.None());

        // Act
        var result = await _jobFactory.CreateAsync(jobParameters);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe(JobTemplateErrors.NotFoundCode);
    }

    [Fact]
    public async Task CreateAsync_BatchJob_ShouldReturnBatchJob_WhenAllJobsAreValid()
    {
        // Arrange
        var templateName = JobTemplateName.Create("TestTemplate").Value;
        var jobTemplate = new JobTemplate
        {
            Name = templateName,
            Version = "1.0",
            Inputs = [new JobTemplateInput { Name = "Input1" }],
            Outputs = [new JobTemplateOutput { Name = "Output1" }],
            Steps =
            [
                new JobTemplateStep { Name = "Step1", ProcessorName = new ProcessorName("namespace", "processor") }
            ]
        };

        var jobParameters = new JobParameters(
            templateName,
            [new JobInputParameter("Input1", "input-uri")],
            [new JobOutputParameter("Output1", "output-uri")],
            [new JobPropertyParameter("Property1", "Value1")]);

        var batchJobParameters = new BatchJobParameters
        {
            Entries = [jobParameters]
        };

        _jobTemplatesRepository.GetAsync(templateName).Returns(Option<JobTemplate>.Some(jobTemplate));

        // Act
        var result = await _jobFactory.CreateAsync(batchJobParameters);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Jobs.ShouldHaveSingleItem();
    }

    [Fact]
    public async Task CreateAsync_BatchJob_ShouldReturnError_WhenAnyJobIsInvalid()
    {
        // Arrange
        var templateName = JobTemplateName.Create("NonExistentTemplate").Value;
        var jobParameters = new JobParameters(
            templateName,
            [],
            [],
            []);

        var batchJobParameters = new BatchJobParameters
        {
            Entries = [jobParameters]
        };

        _jobTemplatesRepository.GetAsync(templateName).Returns(Option<JobTemplate>.None());

        // Act
        var result = await _jobFactory.CreateAsync(batchJobParameters);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe(JobTemplateErrors.NotFoundCode);
    }
}