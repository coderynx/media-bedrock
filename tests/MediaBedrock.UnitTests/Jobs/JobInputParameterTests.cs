using MediaBedrock.Cli.Domain.Jobs.Parameters;
using Shouldly;

namespace MediaBedrock.UnitTests.Jobs;

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

    [Fact]
    public void CreateMultiple_ReturnsJobOutputParameters_WhenInputIsValid()
    {
        // Arrange
        const string input = "name1=uri1,name2=uri2";

        // Act
        var result = JobOutputParameter.CreateMultiple(input);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Length.ShouldBe(2);
        result.Value[0].Name.ShouldBe("name1");
        result.Value[0].Uri.ShouldBe("uri1");
        result.Value[1].Name.ShouldBe("name2");
        result.Value[1].Uri.ShouldBe("uri2");
    }

    [Fact]
    public void CreateMultiple_ReturnsError_WhenOutputInputIsInvalid()
    {
        // Arrange
        const string input = "name1=uri1,name2";

        // Act
        var result = JobOutputParameter.CreateMultiple(input);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldBe(JobParameterErrors.InvalidOutputParameter(input));
    }

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
    public void CreateMultiple_ReturnsError_WhenPropertyInputIsInvalid()
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