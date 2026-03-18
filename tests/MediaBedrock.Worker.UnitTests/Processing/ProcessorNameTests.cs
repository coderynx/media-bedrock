using MediaBedrock.Worker.Domain.Processing;
using MediaBedrock.Worker.Domain.Processing.ValueObjects;
using Shouldly;

namespace MediaBedrock.Worker.UnitTests.Processing;

public sealed class ProcessorNameTests
{
    [Fact]
    public void Create_ShouldReturnProcessorName_WhenFullNameIsValid()
    {
        // Arrange
        const string fullName = "namespace/name";

        // Act
        var createProcessorName = ProcessorName.Create(fullName);

        // Assert
        createProcessorName.IsSuccess.ShouldBeTrue();
        createProcessorName.Value.Namespace.ShouldBe("namespace");
        createProcessorName.Value.Name.ShouldBe("name");
    }

    [Fact]
    public void Create_ShouldReturnError_WhenFullNameIsInvalid()
    {
        // Arrange
        const string fullName = "invalidFullName";

        // Act
        var createProcessorName = ProcessorName.Create(fullName);

        // Assert
        createProcessorName.IsSuccess.ShouldBeFalse();
        createProcessorName.Error.ShouldBe(ProcessorInstanceErrors.InvalidProcessorName(fullName));
    }

    [Fact]
    public void ToString_ShouldReturnCorrectFormat()
    {
        // Arrange
        var createProcessorName = ProcessorName.Create("namespace", "name");

        // Act
        var result = createProcessorName.Value.ToString();

        // Assert
        result.ShouldBe("namespace/name");
    }
}