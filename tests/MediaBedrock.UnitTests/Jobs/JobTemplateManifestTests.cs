using MediaBedrock.Domain.JobTemplates;
using MediaBedrock.Domain.JobTemplates.Manifests;
using MediaBedrock.Domain.Processors;
using Shouldly;

namespace MediaBedrock.UnitTests.Jobs;

public sealed class JobTemplateManifestTests
{
    [Fact]
    public void ToTemplate_ReturnsJobTemplate_WhenManifestIsValid()
    {
        // Arrange
        var firstStep = new JobTemplateManifestStep(
            name: new JobTemplateStepName("step1"),
            processorName: new ProcessorName("namespace", "processor1"),
            inputsMappings: new Dictionary<string, string>().AsReadOnly(),
            outputsMappings: new Dictionary<string, string>().AsReadOnly(),
            properties: new Dictionary<string, string>().AsReadOnly(),
            displayName: "Step1",
            description: "Step1 description");

        var secondStep = new JobTemplateManifestStep(
            name: new JobTemplateStepName("step2"),
            processorName: new ProcessorName("namespace", "processor2"),
            inputsMappings: new Dictionary<string, string>().AsReadOnly(),
            outputsMappings: new Dictionary<string, string>().AsReadOnly(),
            properties: new Dictionary<string, string>().AsReadOnly(),
            displayName: "Step2",
            description: "Step2 description");

        var manifest = new JobTemplateManifest(
            name: new JobTemplateName("Template1"),
            version: new JobTemplateVersion(),
            author: new JobTemplateAuthor(),
            properties: [],
            inputs: [],
            outputs: [],
            steps: [firstStep, secondStep]);

        // Act
        var template = manifest.ToTemplate();

        // Assert
        template.Id.Value.ShouldNotBe(Guid.Empty);
        template.Name.ShouldBe(manifest.Name);
        template.Version.ShouldBe(manifest.Version);
        template.Author.ShouldBe(manifest.Author);
        template.Steps.Count.ShouldBe(2);

        template.Steps[0].Name.ShouldBe(firstStep.Name);
        template.Steps[0].Order.Value.ShouldBe((uint)1);
        template.Steps[0].ProcessorName.ShouldBe(firstStep.ProcessorName);
        template.Steps[0].DisplayName.ShouldBe(firstStep.DisplayName);
        template.Steps[0].Description.ShouldBe(firstStep.Description);
        template.Steps[0].Inputs.ShouldBeEmpty();
        template.Steps[0].Outputs.ShouldBeEmpty();
        template.Steps[0].Properties.ShouldBeEmpty();

        template.Steps[1].Name.ShouldBe(secondStep.Name);
        template.Steps[1].Order.Value.ShouldBe((uint)2);
        template.Steps[1].ProcessorName.ShouldBe(secondStep.ProcessorName);
        template.Steps[1].DisplayName.ShouldBe(secondStep.DisplayName);
        template.Steps[1].Description.ShouldBe(secondStep.Description);
        template.Steps[1].Inputs.ShouldBeEmpty();
        template.Steps[1].Outputs.ShouldBeEmpty();
    }
}