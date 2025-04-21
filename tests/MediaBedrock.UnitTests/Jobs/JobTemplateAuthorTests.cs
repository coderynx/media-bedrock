using MediaBedrock.Cli.Domain.JobTemplates;
using Shouldly;

namespace MediaBedrock.UnitTests.Jobs;

public sealed class JobTemplateAuthorTests
{
    [Fact]
    public void Create_ShouldReturnError_WhenValueIsWhitespace()
    {
        // Arrange.
        const string value = "   ";

        // Act.
        var result = JobTemplateAuthor.Create(value);

        // Assert.
        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe(JobTemplateErrors.InvalidAuthorCode);
    }

    [Fact]
    public void Create_ShouldReturnSuccess_WhenValueIsValid()
    {
        // Arrange.
        const string value = "Valid Author";

        // Act.
        var result = JobTemplateAuthor.Create(value);

        // Assert.
        result.IsSuccess.ShouldBeTrue();
        result.Value.Value.ShouldBe(value);
    }

    [Fact]
    public void Empty_ShouldHaveEmptyValue()
    {
        // Act.
        var emptyAuthor = JobTemplateAuthor.Empty;

        // Assert.
        emptyAuthor.Value.ShouldBe(string.Empty);
    }

    [Fact]
    public void ToString_ShouldReturnValue()
    {
        // Arrange.
        const string value = "Author Name";
        var author = JobTemplateAuthor.Create(value).Value;

        // Act.
        var result = author.ToString();

        // Assert.
        result.ShouldBe(value);
    }
}