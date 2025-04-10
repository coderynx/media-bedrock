using MediaBedrock.Cli.Application.Jobs;
using MediaBedrock.Cli.Domain.Jobs.Parameters;
using MediaBedrock.Cli.Domain.Jobs.Steps;
using MediaBedrock.Cli.Domain.Jobs.Templates;
using Shouldly;

namespace MediaBedrock.UnitTests.Jobs;

public sealed class JobFactoryTests
{
    private readonly JobFactory _jobFactory = new();

    [Fact]
    public void Create_ShouldReturnJob_WhenValidTemplateAndParameters()
    {
        // Arrange
        var template = new JobTemplate
        {
            Name = JobTemplateName.Create("TestTemplate").Value,
            Version = "1.0",
            Inputs = [new JobTemplateInput { Name = "Input1" }],
            Outputs = [new JobTemplateOutput { Name = "Output1" }],
            Steps = [new JobTemplateStep { Name = "Step1", ProcessorName = "namespace/processor" }]
        };

        var parameters = new JobParameters(
            Inputs: [new JobInputParameter("Input1", "uri1")],
            Outputs: [new JobOutputParameter("Output1", "uri2")],
            Properties: [new JobPropertyParameter("Property1", "Value1")]);

        // Act
        var result = _jobFactory.Create(template, parameters);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.TemplateName.ShouldBe(JobTemplateName.Create("TestTemplate").Value);
        result.Value.Inputs.ShouldHaveSingleItem().Name.ShouldBe("Input1");
        result.Value.Outputs.ShouldHaveSingleItem().Name.ShouldBe("Output1");
        result.Value.Steps.ShouldHaveSingleItem().Name.ShouldBe(JobStepName.Create("Step1").Value);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenInputParameterNotFound()
    {
        // Arrange
        var template = new JobTemplate
        {
            Name = JobTemplateName.Create("TestTemplate").Value,
            Version = "1.0",
            Inputs = [new JobTemplateInput { Name = "Input1" }]
        };

        var parameters = new JobParameters(
            Inputs: [new JobInputParameter("InvalidInput", "uri1")],
            Outputs: [],
            Properties: []);

        // Act
        var result = _jobFactory.Create(template, parameters);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(JobParameterErrors.InputParameterNotFound("InvalidInput"));
    }

    [Fact]
    public void Create_ShouldReturnError_WhenOutputParameterNotFound()
    {
        // Arrange
        var template = new JobTemplate
        {
            Name = JobTemplateName.Create("TestTemplate").Value,
            Version = "1.0",
            Outputs = [new JobTemplateOutput { Name = "Output1" }]
        };

        var parameters = new JobParameters(
            Inputs: [],
            Outputs: [new JobOutputParameter("InvalidOutput", "uri2")],
            Properties: []);

        // Act
        var result = _jobFactory.Create(template, parameters);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(JobParameterErrors.OutputParameterNotFound("InvalidOutput"));
    }
}