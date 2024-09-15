using MediaBedrock.Cli.Application.Jobs;
using MediaBedrock.Cli.Domain.Jobs;
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

        var inputs = new[] { new JobInputParameter("Input1", "uri1") };
        var outputs = new[] { new JobOutputParameter("Output1", "uri2") };
        var properties = new[] { new JobPropertyParameter("Property1", "Value1") };

        // Act
        var result = _jobFactory.Create(template, inputs, outputs, properties);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.TemplateName.ShouldBe(JobTemplateName.Create("TestTemplate").Value);
        result.Value.Inputs.ShouldHaveSingleItem().Name.ShouldBe("Input1");
        result.Value.Outputs.ShouldHaveSingleItem().Name.ShouldBe("Output1");
        result.Value.Steps.ShouldHaveSingleItem().Name.ShouldBe("Step1");
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

        var inputs = new[] { new JobInputParameter("InvalidInput", "uri1") };
        var outputs = Array.Empty<JobOutputParameter>();
        var properties = Array.Empty<JobPropertyParameter>();

        // Act
        var result = _jobFactory.Create(template, inputs, outputs, properties);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(JobErrors.InputParameterNotFound("InvalidInput"));
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

        var inputs = Array.Empty<JobInputParameter>();
        var outputs = new[] { new JobOutputParameter("InvalidOutput", "uri2") };
        var properties = Array.Empty<JobPropertyParameter>();

        // Act
        var result = _jobFactory.Create(template, inputs, outputs, properties);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(JobErrors.OutputParameterNotFound("InvalidOutput"));
    }
}