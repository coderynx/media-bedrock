using MediaBedrock.Cli.Domain.JobTemplates;
using Shouldly;

namespace MediaBedrock.UnitTests.Jobs;

public sealed class JobTemplateVersionTests
{
    [Fact]
    public void Create_ReturnsCreatedResult_WhenValueIsValidSemver()
    {
        // Arrange
        const string validSemver = "1.0.0";

        // Act
        var result = JobTemplateVersion.Create(validSemver);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Value.ShouldBe(validSemver);
    }

    [Fact]
    public void Create_ReturnsError_WhenValueIsEmpty()
    {
        // Arrange
        const string emptyValue = "";

        // Act
        var result = JobTemplateVersion.Create(emptyValue);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(JobTemplateErrors.InvalidVersion(emptyValue));
    }

    [Fact]
    public void Create_ReturnsError_WhenValueIsNotSemverCompliant()
    {
        // Arrange
        const string invalidSemver = "1.0";

        // Act
        var result = JobTemplateVersion.Create(invalidSemver);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(JobTemplateErrors.InvalidVersion(invalidSemver));
    }

    [Fact]
    public void Create_ReturnsError_WhenValueContainsInvalidCharacters()
    {
        // Arrange
        const string invalidSemver = "1.0.0-@lpha";

        // Act
        var result = JobTemplateVersion.Create(invalidSemver);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(JobTemplateErrors.InvalidVersion(invalidSemver));
    }

    [Fact]
    public void Create_ReturnsCreatedResult_WhenValueHasPreReleaseAndBuildMetadata()
    {
        // Arrange
        const string validSemver = "1.0.0-alpha+001";

        // Act
        var result = JobTemplateVersion.Create(validSemver);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Value.ShouldBe(validSemver);
    }
}