using MediaBedrock.Controller.Domain.Jobs.Parameters;
using Shouldly;

namespace MediaBedrock.Controller.UnitTests.Jobs;

public class JobPropertyParameterTests
{
    [Fact]
    public void CreateMultiple_ReturnsJobPropertyParameters_WhenInputIsValid()
    {
        // Arrange
        const string input = "name1=value1,name2=value2";

        // Act
        var result = JobPropertyParameter.CreateMultiple(input);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Length.ShouldBe(2);
        result.Value[0].Name.ShouldBe("name1");
        result.Value[0].Value.ShouldBe("value1");
        result.Value[1].Name.ShouldBe("name2");
        result.Value[1].Value.ShouldBe("value2");
    }

    [Fact]
    public void CreateMultiple_ReturnsError_WhenInputIsInvalid()
    {
        // Arrange
        const string input = "name1=value1,name2";

        // Act
        var result = JobPropertyParameter.CreateMultiple(input);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldBe(JobParameterErrors.InvalidPropertyParameter(input));
    }
}