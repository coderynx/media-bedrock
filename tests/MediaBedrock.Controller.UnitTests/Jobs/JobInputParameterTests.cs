using MediaBedrock.Controller.Domain.Jobs.Parameters;
using Shouldly;

namespace MediaBedrock.Controller.UnitTests.Jobs;

public sealed class JobInputParameterTests
{
    [Fact]
    public void CreateMultiple_ReturnsJobInputParameters_WhenInputIsValid()
    {
        // Arrange
        const string input = "name1=uri1,name2=uri2";

        // Act
        var result = JobInputParameter.CreateMultiple(input);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Length.ShouldBe(2);
        result.Value[0].Name.ShouldBe("name1");
        result.Value[0].Uri.ShouldBe("uri1");
        result.Value[1].Name.ShouldBe("name2");
        result.Value[1].Uri.ShouldBe("uri2");
    }

    [Fact]
    public void CreateMultiple_ThrowsArgumentException_WhenInputIsInvalid()
    {
        // Arrange
        const string input = "name1=uri1,name2";

        // Act
        var result = JobInputParameter.CreateMultiple(input);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldBe(JobParameterErrors.InvalidInputParameter(input));
    }
}