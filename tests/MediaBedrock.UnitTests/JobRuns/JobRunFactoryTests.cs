using AutoFixture;
using Coderynx.Functional.Results;
using Coderynx.Functional.Results.Errors;
using MediaBedrock.Domain.JobAssets;
using MediaBedrock.Domain.JobAssets.Interfaces;
using MediaBedrock.Domain.JobRuns;
using MediaBedrock.Domain.Jobs;
using MediaBedrock.Domain.Jobs.Parameters;
using MediaBedrock.Domain.JobTemplates;
using MediaBedrock.Domain.Processors;
using MediaBedrock.Sdk.Processors;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;

namespace MediaBedrock.UnitTests.JobRuns;

public sealed class JobRunFactoryTests
{
    private readonly IFixture _fixture;
    private readonly JobRunFactory _jobRunFactory;
    private readonly IMediaInformationRetriever _mediaInformationRetriever;

    public JobRunFactoryTests()
    {
        _fixture = new Fixture();
        _mediaInformationRetriever = Substitute.For<IMediaInformationRetriever>();

        var logger = Substitute.For<ILogger<JobRunFactory>>();
        _jobRunFactory = new JobRunFactory(_mediaInformationRetriever, logger);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnSuccess_WhenJobHasValidInputsOutputsAndSteps()
    {
        // Arrange
        var mediaInformation = _fixture.Create<MediaInformation>();
        var job = CreateValidJob();

        _mediaInformationRetriever
            .GetMediaInfoAsync(Arg.Any<string>())
            .Returns(Task.FromResult(Result.Created(mediaInformation)));

        // Act
        var result = await _jobRunFactory.CreateAsync(job);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Job.ShouldBe(job);
        result.Value.Status.ShouldBe(JobRunStatus.Pending);
        result.Value.AssetsPool.ShouldNotBeEmpty();
        result.Value.Steps.ShouldNotBeEmpty();
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnFailure_WhenMediaInformationRetrievalFails()
    {
        // Arrange
        var job = CreateValidJob();
        var error = Error.NotFound("MediaInfo.Failed", "Failed to retrieve media information");

        _mediaInformationRetriever
            .GetMediaInfoAsync(Arg.Any<string>())
            .Returns(Task.FromResult(new Result<MediaInformation>(error)));

        // Act
        var result = await _jobRunFactory.CreateAsync(job);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(error);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateInputAssets_WhenJobHasInputs()
    {
        // Arrange
        var mediaInformation = _fixture.Create<MediaInformation>();
        var job = CreateJobWithInputs(2);

        _mediaInformationRetriever
            .GetMediaInfoAsync(Arg.Any<string>())
            .Returns(Task.FromResult(Result.Created(mediaInformation)));

        // Act
        var result = await _jobRunFactory.CreateAsync(job);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.AssetsPool.Count(a => a.Kind == JobAssetKind.Input).ShouldBe(2);
        await _mediaInformationRetriever.Received(2).GetMediaInfoAsync(Arg.Any<string>());
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateOutputAssets_WhenJobHasOutputs()
    {
        // Arrange
        var mediaInformation = _fixture.Create<MediaInformation>();
        var job = CreateJobWithOutputs(3);

        _mediaInformationRetriever
            .GetMediaInfoAsync(Arg.Any<string>())
            .Returns(Task.FromResult(Result.Created(mediaInformation)));

        // Act
        var result = await _jobRunFactory.CreateAsync(job);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.AssetsPool.Count(a => a.Kind == JobAssetKind.Output).ShouldBe(3);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateMezzanineAssets_WhenStepsRequireThem()
    {
        // Arrange
        var mediaInformation = _fixture.Create<MediaInformation>();
        var job = CreateJobWithStepsRequiringMezzanines();

        _mediaInformationRetriever
            .GetMediaInfoAsync(Arg.Any<string>())
            .Returns(Task.FromResult(Result.Created(mediaInformation)));

        // Act
        var result = await _jobRunFactory.CreateAsync(job);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.AssetsPool.Any(a => a.Kind == JobAssetKind.Mezzanine).ShouldBeTrue();
    }

    [Fact]
    public async Task CreateAsync_ShouldNotCreateDuplicateAssets_WhenMultipleStepsUsesSameAssetName()
    {
        // Arrange
        var mediaInformation = _fixture.Create<MediaInformation>();
        var job = CreateJobWithDuplicateAssetReferences();

        _mediaInformationRetriever
            .GetMediaInfoAsync(Arg.Any<string>())
            .Returns(Task.FromResult(Result.Created(mediaInformation)));

        // Act
        var result = await _jobRunFactory.CreateAsync(job);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        var assetNames = result.Value.AssetsPool.Select(a => a.Name.Value).ToList();
        assetNames.Count.ShouldBe(assetNames.Distinct().Count());
    }

    [Fact]
    public async Task CreateAsync_ShouldAddAllStepsToJobRun_WhenStepsAreValid()
    {
        // Arrange
        var mediaInformation = _fixture.Create<MediaInformation>();
        var job = CreateJobWithMultipleSteps(3);

        _mediaInformationRetriever
            .GetMediaInfoAsync(Arg.Any<string>())
            .Returns(Task.FromResult(Result.Created(mediaInformation)));

        // Act
        var result = await _jobRunFactory.CreateAsync(job);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Steps.Count.ShouldBe(3);
    }

    [Fact]
    public async Task CreateAsync_ShouldHandleJobWithNoInputs()
    {
        // Arrange
        var job = CreateJobWithNoInputs();

        // Act
        var result = await _jobRunFactory.CreateAsync(job);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.AssetsPool.Count(a => a.Kind == JobAssetKind.Input).ShouldBe(0);
        await _mediaInformationRetriever.DidNotReceive().GetMediaInfoAsync(Arg.Any<string>());
    }

    [Fact]
    public async Task CreateAsync_ShouldHandleJobWithNoOutputs()
    {
        // Arrange
        var mediaInformation = _fixture.Create<MediaInformation>();
        var job = CreateJobWithNoOutputs();

        _mediaInformationRetriever
            .GetMediaInfoAsync(Arg.Any<string>())
            .Returns(Task.FromResult(Result.Created(mediaInformation)));

        // Act
        var result = await _jobRunFactory.CreateAsync(job);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.AssetsPool.Count(a => a.Kind == JobAssetKind.Output).ShouldBe(0);
    }

    [Fact]
    public async Task CreateAsync_ShouldHandleJobWithNoSteps()
    {
        // Arrange
        var mediaInformation = _fixture.Create<MediaInformation>();
        var job = CreateJobWithNoSteps();

        _mediaInformationRetriever
            .GetMediaInfoAsync(Arg.Any<string>())
            .Returns(Task.FromResult(Result.Created(mediaInformation)));

        // Act
        var result = await _jobRunFactory.CreateAsync(job);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Steps.Count.ShouldBe(0);
    }

    private static Job CreateValidJob()
    {
        var template = JobTemplate.Create(
            name: new JobTemplateName("TestTemplate"),
            version: new JobTemplateVersion(),
            author: new JobTemplateAuthor(),
            inputs: [new JobTemplateInput("input1")],
            outputs: [new JobTemplateOutput("output1")],
            properties: []);

        var step = JobTemplateStep.Create(
            template: template,
            name: new JobTemplateStepName("step1"),
            processorName: ProcessorName.Create("test/processor").Value,
            inputs: [new JobTemplateStepInput("input1", "input1")],
            outputs: [new JobTemplateStepOutput("output1", "output1")],
            properties: []);

        template.AddStepRange([step]);

        var parameters = new JobParameters(
            TemplateName: new JobTemplateName("TestTemplate"),
            Inputs: [new JobInputParameter("input1", "test://input1.mp4")],
            Outputs: [new JobOutputParameter("output1", "/tmp/output1.mp4")],
            Properties: []);

        var jobFactory = new JobFactory();
        return jobFactory.Create(template, parameters).Value;
    }

    private static Job CreateJobWithInputs(int inputCount)
    {
        var inputs = Enumerable.Range(1, inputCount)
            .Select(i => new JobTemplateInput($"input{i}"))
            .ToList();

        var template = JobTemplate.Create(
            name: new JobTemplateName("TestTemplate"),
            version: new JobTemplateVersion(),
            author: new JobTemplateAuthor(),
            inputs: inputs,
            outputs: [],
            properties: []);

        var inputParameters = Enumerable.Range(1, inputCount)
            .Select(i => new JobInputParameter($"input{i}", $"test://input{i}.mp4"))
            .ToArray();

        var parameters = new JobParameters(
            TemplateName: new JobTemplateName("TestTemplate"),
            Inputs: inputParameters,
            Outputs: [],
            Properties: []);

        var jobFactory = new JobFactory();
        return jobFactory.Create(template, parameters).Value;
    }

    private static Job CreateJobWithOutputs(int outputCount)
    {
        var outputs = Enumerable.Range(1, outputCount)
            .Select(i => new JobTemplateOutput($"output{i}"))
            .ToList();

        var template = JobTemplate.Create(
            name: new JobTemplateName("TestTemplate"),
            version: new JobTemplateVersion(),
            author: new JobTemplateAuthor(),
            inputs: [],
            outputs: outputs,
            properties: []);

        var outputParameters = Enumerable.Range(1, outputCount)
            .Select(i => new JobOutputParameter($"output{i}", $"/tmp/output{i}.mp4"))
            .ToArray();

        var parameters = new JobParameters(
            TemplateName: new JobTemplateName("TestTemplate"),
            Inputs: [],
            Outputs: outputParameters,
            Properties: []);

        var jobFactory = new JobFactory();
        return jobFactory.Create(template, parameters).Value;
    }

    private static Job CreateJobWithStepsRequiringMezzanines()
    {
        var template = JobTemplate.Create(
            name: new JobTemplateName("TestTemplate"),
            version: new JobTemplateVersion(),
            author: new JobTemplateAuthor(),
            inputs: [],
            outputs: [],
            properties: []);

        var step = JobTemplateStep.Create(
            template: template,
            name: new JobTemplateStepName("step1"),
            processorName: ProcessorName.Create("test/processor").Value,
            inputs: [new JobTemplateStepInput("mezzanine1", "mezzanine1")],
            outputs: [new JobTemplateStepOutput("mezzanine2", "mezzanine2")],
            properties: []);

        template.AddStepRange([step]);

        var parameters = new JobParameters(
            TemplateName: new JobTemplateName("TestTemplate"),
            Inputs: [],
            Outputs: [],
            Properties: []);

        var jobFactory = new JobFactory();
        return jobFactory.Create(template, parameters).Value;
    }

    private static Job CreateJobWithDuplicateAssetReferences()
    {
        var template = JobTemplate.Create(
            name: new JobTemplateName("TestTemplate"),
            version: new JobTemplateVersion(),
            author: new JobTemplateAuthor(),
            inputs: [],
            outputs: [],
            properties: []);

        var step1 = JobTemplateStep.Create(
            template: template,
            name: new JobTemplateStepName("step1"),
            processorName: ProcessorName.Create("test/processor").Value,
            inputs: [new JobTemplateStepInput("shared_asset", "shared_asset")],
            outputs: [new JobTemplateStepOutput("output1", "output1")],
            properties: []);

        var step2 = JobTemplateStep.Create(
            template: template,
            name: new JobTemplateStepName("step2"),
            processorName: ProcessorName.Create("test/processor").Value,
            inputs: [new JobTemplateStepInput("shared_asset", "shared_asset")],
            outputs: [new JobTemplateStepOutput("output2", "output2")],
            properties: []);

        template.AddStepRange([step1, step2]);

        var parameters = new JobParameters(
            TemplateName: new JobTemplateName("TestTemplate"),
            Inputs: [],
            Outputs: [],
            Properties: []);

        var jobFactory = new JobFactory();
        return jobFactory.Create(template, parameters).Value;
    }

    private static Job CreateJobWithMultipleSteps(int stepCount)
    {
        var template = JobTemplate.Create(
            name: new JobTemplateName("TestTemplate"),
            version: new JobTemplateVersion(),
            author: new JobTemplateAuthor(),
            inputs: [],
            outputs: [],
            properties: []);

        var steps = new List<JobTemplateStep>();
        for (var i = 1; i <= stepCount; i++)
        {
            var step = JobTemplateStep.Create(
                template: template,
                name: new JobTemplateStepName($"step{i}"),
                processorName: ProcessorName.Create("test/processor").Value,
                inputs: [],
                outputs: [],
                properties: []);

            steps.Add(step);
        }

        template.AddStepRange(steps);

        var parameters = new JobParameters(
            TemplateName: new JobTemplateName("TestTemplate"),
            Inputs: [],
            Outputs: [],
            Properties: []);

        var jobFactory = new JobFactory();
        return jobFactory.Create(template, parameters).Value;
    }

    private static Job CreateJobWithNoInputs()
    {
        var template = JobTemplate.Create(
            name: new JobTemplateName("TestTemplate"),
            version: new JobTemplateVersion(),
            author: new JobTemplateAuthor(),
            inputs: [],
            outputs: [new JobTemplateOutput("output1")],
            properties: []);

        var parameters = new JobParameters(
            TemplateName: new JobTemplateName("TestTemplate"),
            Inputs: [],
            Outputs: [new JobOutputParameter("output1", "/tmp/output1.mp4")],
            Properties: []);

        var jobFactory = new JobFactory();
        return jobFactory.Create(template, parameters).Value;
    }

    private static Job CreateJobWithNoOutputs()
    {
        var template = JobTemplate.Create(
            name: new JobTemplateName("TestTemplate"),
            version: new JobTemplateVersion(),
            author: new JobTemplateAuthor(),
            inputs: [new JobTemplateInput("input1")],
            outputs: [],
            properties: []);

        var parameters = new JobParameters(
            TemplateName: new JobTemplateName("TestTemplate"),
            Inputs: [new JobInputParameter("input1", "test://input1.mp4")],
            Outputs: [],
            Properties: []);

        var jobFactory = new JobFactory();
        return jobFactory.Create(template, parameters).Value;
    }

    private static Job CreateJobWithNoSteps()
    {
        var template = JobTemplate.Create(
            name: new JobTemplateName("TestTemplate"),
            version: new JobTemplateVersion(),
            author: new JobTemplateAuthor(),
            inputs: [new JobTemplateInput("input1")],
            outputs: [new JobTemplateOutput("output1")],
            properties: []);

        var parameters = new JobParameters(
            TemplateName: new JobTemplateName("TestTemplate"),
            Inputs: [new JobInputParameter("input1", "test://input1.mp4")],
            Outputs: [new JobOutputParameter("output1", "/tmp/output1.mp4")],
            Properties: []);

        var jobFactory = new JobFactory();
        return jobFactory.Create(template, parameters).Value;
    }
}