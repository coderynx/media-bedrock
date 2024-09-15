using MediaBedrock.Cli.Domain.Jobs.Processors;
using Shouldly;

namespace MediaBedrock.UnitTests.Jobs.Processors;

public sealed class ProcessorNameTests
{
    [Fact]
    public void Create_ShouldReturnProcessorName_WhenFullNameIsValid()
    {
        // Arrange
        const string fullName = "namespace/name";

        // Act
        var result = ProcessorName.Create(fullName);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Namespace.ShouldBe("namespace");
        result.Value.Name.ShouldBe("name");
    }

    [Fact]
    public void Create_ShouldReturnError_WhenFullNameIsInvalid()
    {
        // Arrange
        const string fullName = "invalidFullName";

        // Act
        var result = ProcessorName.Create(fullName);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldBe(ProcessorErrors.InvalidName(fullName));
    }

    [Fact]
    public void ToString_ShouldReturnCorrectFormat()
    {
        // Arrange
        var processorName = new ProcessorName("namespace", "name");

        // Act
        var result = processorName.ToString();

        // Assert
        result.ShouldBe("namespace/name");
    }
}