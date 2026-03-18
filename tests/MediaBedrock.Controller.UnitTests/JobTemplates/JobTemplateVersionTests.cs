using Coderynx.Functional.Results.Errors;
using MediaBedrock.Controller.Domain.JobTemplates;
using Shouldly;

namespace MediaBedrock.Controller.UnitTests.JobTemplates;

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
    public void New_ReturnsJobTemplateVersion_WhenValueIsValidSemver()
    {
        // Arrange
        const string validSemver = "1.0.0";

        // Act
        var result = new JobTemplateVersion(validSemver);

        // Assert
        result.Value.ShouldBe(validSemver);
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
    public void New_ThrowsErrorException_WhenValueIsEmpty()
    {
        // Arrange
        const string emptyValue = "";

        // Act
        var exception = Record.Exception(() => new JobTemplateVersion(emptyValue)) as ErrorException;

        // Assert
        exception.ShouldNotBeNull();
        exception.Error.Kind.ShouldBe(ErrorKind.InvalidInput);
        exception.Error.Code.ShouldBe(JobTemplateErrorCodes.InvalidVersion);
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
    public void New_ThrowsErrorException_WhenValueIsNotSemverCompliant()
    {
        // Arrange
        const string invalidSemver = "1.0";

        // Act
        var exception = Record.Exception(() => new JobTemplateVersion(invalidSemver)) as ErrorException;

        // Assert
        exception.ShouldNotBeNull();
        exception.Error.Kind.ShouldBe(ErrorKind.InvalidInput);
        exception.Error.Code.ShouldBe(JobTemplateErrorCodes.InvalidVersion);
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
    public void New_ThrowsErrorException_WhenValueContainsInvalidCharacters()
    {
        // Arrange
        const string invalidSemver = "1.0.0-@lpha";

        // Act
        var exception = Record.Exception(() => new JobTemplateVersion(invalidSemver)) as ErrorException;

        // Assert
        exception.ShouldNotBeNull();
        exception.Error.Kind.ShouldBe(ErrorKind.InvalidInput);
        exception.Error.Code.ShouldBe(JobTemplateErrorCodes.InvalidVersion);
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

    [Fact]
    public void New_ReturnsJobTemplateVersion_WhenValueHasPreReleaseAndBuildMetadata()
    {
        // Arrange
        const string validSemver = "1.0.0-alpha+001";

        // Act
        var result = new JobTemplateVersion(validSemver);

        // Assert
        result.Value.ShouldBe(validSemver);
    }
}