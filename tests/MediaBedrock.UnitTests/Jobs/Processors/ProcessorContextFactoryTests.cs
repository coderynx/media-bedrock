using AutoFixture;
using Coderynx.Functional.Results;
using Coderynx.Functional.Results.Errors;
using MediaBedrock.Application.Jobs;
using MediaBedrock.Domain.JobAssets;
using MediaBedrock.Domain.JobRuns;
using MediaBedrock.Domain.Jobs;
using MediaBedrock.Domain.Jobs.Parameters;
using MediaBedrock.Domain.Jobs.Steps;
using MediaBedrock.Domain.JobTemplates;
using MediaBedrock.Domain.Processors;
using MediaBedrock.Domain.Processors.Interfaces;
using MediaBedrock.Sdk.Processors;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Shouldly;

namespace MediaBedrock.UnitTests.Jobs.Processors;

public sealed class ProcessorContextFactoryTests
{
    private readonly IFixture _fixture;
    private readonly ProcessorContextFactory _processorContextFactory;
    private readonly IProcessorProvider _processorProvider;

    public ProcessorContextFactoryTests()
    {
        _fixture = new Fixture();
        _processorProvider = Substitute.For<IProcessorProvider>();
        var loggerFactory = Substitute.For<ILoggerFactory>();

        loggerFactory.CreateLogger(null!).ReturnsForAnyArgs(NullLogger.Instance);

        _processorContextFactory = new ProcessorContextFactory(_processorProvider, loggerFactory);
    }

    [Fact]
    public void Create_ShouldReturnSuccess_WhenAllDependenciesAreValid()
    {
        // Arrange
        var processorType = typeof(TestProcessor);
        var jobRun = CreateJobRunWithValidStep();
        var jobStepName = new JobStepName("step1");
        var processorConfiguration = CreateValidProcessorConfiguration();

        _processorProvider
            .ResolveConfiguration(Arg.Any<ProcessorName>())
            .Returns(Result.Created(processorConfiguration));

        // Act
        var result = _processorContextFactory.Create(processorType, jobRun, jobStepName);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Inputs.ShouldNotBeEmpty();
        result.Value.Outputs.ShouldNotBeEmpty();
        result.Value.Properties.ShouldNotBeEmpty();
    }

    [Fact]
    public void Create_ShouldReturnError_WhenStepNotFound()
    {
        // Arrange
        var processorType = typeof(TestProcessor);
        var jobRun = CreateJobRunWithValidStep();
        var nonExistentStepName = new JobStepName("nonexistent");

        // Act
        var result = _processorContextFactory.Create(processorType, jobRun, nonExistentStepName);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe(JobErrorCodes.StepNotFound);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenProcessorConfigurationResolutionFails()
    {
        // Arrange
        var processorType = typeof(TestProcessor);
        var jobRun = CreateJobRunWithValidStep();
        var jobStepName = new JobStepName("step1");
        var error = Error.NotFound("Processor.NotFound", "Processor configuration not found");

        _processorProvider
            .ResolveConfiguration(Arg.Any<ProcessorName>())
            .Returns(new Result<ProcessorConfiguration>(error));

        // Act
        var result = _processorContextFactory.Create(processorType, jobRun, jobStepName);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(error);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenInputAssetNotFound()
    {
        // Arrange
        var processorType = typeof(TestProcessor);
        var jobRun = CreateJobRunWithMissingInputAsset();
        var jobStepName = new JobStepName("step1");
        var processorConfiguration = CreateValidProcessorConfiguration();

        _processorProvider
            .ResolveConfiguration(Arg.Any<ProcessorName>())
            .Returns(Result.Created(processorConfiguration));

        // Act
        var result = _processorContextFactory.Create(processorType, jobRun, jobStepName);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe(JobAssetErrorCodes.NotFound);
    }

    [Fact]
    public void Create_ShouldIncludeStepProperties_WhenStepHasProperties()
    {
        // Arrange
        var processorType = typeof(TestProcessor);
        var jobRun = CreateJobRunWithStepProperties();
        var jobStepName = new JobStepName("step1");
        var processorConfiguration = CreateValidProcessorConfiguration();

        _processorProvider
            .ResolveConfiguration(Arg.Any<ProcessorName>())
            .Returns(Result.Created(processorConfiguration));

        // Act
        var result = _processorContextFactory.Create(processorType, jobRun, jobStepName);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Properties.Count.ShouldBeGreaterThan(0);
        result.Value.Properties.Any(p => p.Name == "stepProperty1").ShouldBeTrue();
        result.Value.Properties.Any(p => p.Name == "configProperty1").ShouldBeTrue();
    }

    [Fact]
    public void Create_ShouldCreateTemporaryPath_WhenOutputAssetNotFound()
    {
        // Arrange
        var processorType = typeof(TestProcessor);
        var jobRun = CreateJobRunWithMissingOutputAsset();
        var jobStepName = new JobStepName("step1");
        var processorConfiguration = CreateValidProcessorConfiguration();

        _processorProvider
            .ResolveConfiguration(Arg.Any<ProcessorName>())
            .Returns(Result.Created(processorConfiguration));

        // Act
        var result = _processorContextFactory.Create(processorType, jobRun, jobStepName);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Outputs.ShouldNotBeEmpty();
        var output = result.Value.Outputs.First();
        output.GetAsFilePath().ShouldContain("temp");
        output.GetAsFilePath().ShouldContain(jobRun.Job.Id.ToString());
    }

    [Fact]
    public void Create_ShouldHandleMultipleInputsAndOutputs()
    {
        // Arrange
        var processorType = typeof(TestProcessor);
        var jobRun = CreateJobRunWithMultipleInputsAndOutputs();
        var jobStepName = new JobStepName("step1");
        var processorConfiguration = CreateValidProcessorConfiguration();

        _processorProvider
            .ResolveConfiguration(Arg.Any<ProcessorName>())
            .Returns(Result.Created(processorConfiguration));

        // Act
        var result = _processorContextFactory.Create(processorType, jobRun, jobStepName);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Inputs.Count.ShouldBe(2);
        result.Value.Outputs.Count.ShouldBe(2);
    }

    [Fact]
    public void Create_ShouldHandleStepWithNoInputsOrOutputs()
    {
        // Arrange
        var processorType = typeof(TestProcessor);
        var jobRun = CreateJobRunWithNoInputsOrOutputs();
        var jobStepName = new JobStepName("step1");
        var processorConfiguration = CreateValidProcessorConfiguration();

        _processorProvider
            .ResolveConfiguration(Arg.Any<ProcessorName>())
            .Returns(Result.Created(processorConfiguration));

        // Act
        var result = _processorContextFactory.Create(processorType, jobRun, jobStepName);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Inputs.ShouldBeEmpty();
        result.Value.Outputs.ShouldBeEmpty();
        result.Value.Properties.ShouldNotBeEmpty(); // Should still have config properties
    }

    private JobRun CreateJobRunWithValidStep()
    {
        var job = CreateTestJob();
        var jobRun = JobRun.Create(job);

        // Create input asset
        var inputAsset = CreateInputAsset(jobRun, "input1", "test://input1.mp4");
        jobRun.AddAsset(inputAsset);

        // Create job step and then job run step
        var jobStep = CreateJobStep("step1", ["input1"], ["output1"], ["stepProperty1:stepValue1"]);
        jobRun.AddStep(jobStep);

        return jobRun;
    }

    private JobRun CreateJobRunWithMissingInputAsset()
    {
        var job = CreateTestJob();
        var jobRun = JobRun.Create(job);

        // Create job step with input that doesn't exist
        var jobStep = CreateJobStep("step1", ["nonexistent"], ["output1"], []);
        jobRun.AddStep(jobStep);

        return jobRun;
    }

    private JobRun CreateJobRunWithStepProperties()
    {
        var job = CreateTestJob();
        var jobRun = JobRun.Create(job);

        // Create input asset
        var inputAsset = CreateInputAsset(jobRun, "input1", "test://input1.mp4");
        jobRun.AddAsset(inputAsset);

        // Create job step with properties
        var jobStep = CreateJobStep("step1", ["input1"], ["output1"],
            ["stepProperty1:stepValue1", "stepProperty2:stepValue2"]);
        jobRun.AddStep(jobStep);

        return jobRun;
    }

    private JobRun CreateJobRunWithMissingOutputAsset()
    {
        var job = CreateTestJob();
        var jobRun = JobRun.Create(job);

        // Create input asset
        var inputAsset = CreateInputAsset(jobRun, "input1", "test://input1.mp4");
        jobRun.AddAsset(inputAsset);

        // Create job step with output that doesn't exist
        var jobStep = CreateJobStep("step1", ["input1"], ["missing_output"], []);
        jobRun.AddStep(jobStep);

        return jobRun;
    }

    private JobRun CreateJobRunWithExistingOutputAsset()
    {
        var job = CreateTestJob();
        var jobRun = JobRun.Create(job);

        // Create input asset
        var inputAsset = CreateInputAsset(jobRun, "input1", "test://input1.mp4");
        jobRun.AddAsset(inputAsset);

        // Create output asset
        var outputAsset = CreateOutputAsset(jobRun, "output1", "test://output1.mp4");
        jobRun.AddAsset(outputAsset);

        // Create job step
        var jobStep = CreateJobStep("step1", ["input1"], ["output1"], []);
        jobRun.AddStep(jobStep);

        return jobRun;
    }

    private JobRun CreateJobRunWithMultipleInputsAndOutputs()
    {
        var job = CreateTestJob();
        var jobRun = JobRun.Create(job);

        // Create multiple input assets
        var inputAsset1 = CreateInputAsset(jobRun, "input1", "test://input1.mp4");
        var inputAsset2 = CreateInputAsset(jobRun, "input2", "test://input2.mp4");
        jobRun.AddAsset(inputAsset1);
        jobRun.AddAsset(inputAsset2);

        // Create job step with multiple inputs and outputs
        var jobStep = CreateJobStep("step1", ["input1", "input2"], ["output1", "output2"], []);
        jobRun.AddStep(jobStep);

        return jobRun;
    }

    private JobRun CreateJobRunWithNoInputsOrOutputs()
    {
        var job = CreateTestJob();
        var jobRun = JobRun.Create(job);

        // Create job step with no inputs or outputs
        var jobStep = CreateJobStep("step1", [], [], []);
        jobRun.AddStep(jobStep);

        return jobRun;
    }

    private Job CreateTestJob()
    {
        var template = JobTemplate.Create(
            name: new JobTemplateName("TestTemplate"),
            version: new JobTemplateVersion(),
            author: new JobTemplateAuthor(),
            inputs: [],
            outputs: [],
            properties: []);

        var parameters = new JobParameters(
            TemplateName: new JobTemplateName("TestTemplate"),
            Inputs: [],
            Outputs: [],
            Properties: []);

        var jobFactory = new JobFactory();
        return jobFactory.Create(template, parameters).Value;
    }

    private JobStep CreateJobStep(string stepName, string[] inputNames, string[] outputNames, string[] properties)
    {
        var job = CreateTestJob();

        var inputs = inputNames.Select(name => JobStepInput.Create(name, new JobAssetName(name)).Value).ToList();
        var outputs = outputNames.Select(name => JobStepOutput.Create(name, new JobAssetName(name)).Value).ToList();
        var stepProperties = properties.Select(prop =>
        {
            var parts = prop.Split(':');
            return JobStepProperty.Create(parts[0], parts.Length > 1 ? parts[1] : "");
        }).ToList();

        return JobStep.Create(
            job: job,
            order: new JobStepOrder(1),
            name: new JobStepName(stepName),
            processorName: ProcessorName.Create("test/processor").Value,
            properties: stepProperties,
            inputs: inputs,
            outputs: outputs);
    }

    private JobAsset CreateInputAsset(JobRun jobRun, string name, string uri)
    {
        var mediaInformation = _fixture.Create<MediaInformation>();
        return JobAsset.CreateInput(
            jobRun: jobRun,
            name: new JobAssetName(name),
            uri: uri,
            mediaInformation: mediaInformation).Value;
    }

    private JobAsset CreateOutputAsset(JobRun jobRun, string name, string uri)
    {
        return JobAsset.CreateOutput(
            jobRun: jobRun,
            name: new JobAssetName(name),
            uri: uri).Value;
    }

    private ProcessorConfiguration CreateValidProcessorConfiguration()
    {
        return new ProcessorConfiguration
        {
            Name = "test-processor",
            PluginPath = "/path/to/plugin",
            Settings = new Dictionary<string, string?>
            {
                { "configProperty1", "configValue1" },
                { "configProperty2", "configValue2" }
            }
        };
    }

    private class TestProcessor;
}