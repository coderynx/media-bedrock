using Cocona;
using MediaBedrock.Controller.Presentation.JobTemplates.Contracts;
using MediaBedrock.Controller.Presentation.JobTemplates.Converters;
using MediaBedrock.Controller.Presentation.JobTemplates.Mappers;
using Spectre.Console;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace MediaBedrock.Controller.Presentation.JobTemplates;

internal sealed class JobTemplatesCommands
{
    private readonly IDeserializer _deserializer = new DeserializerBuilder()
        .WithNamingConvention(UnderscoredNamingConvention.Instance)
        .WithTypeConverter(new ReadOnlyDictionaryStringStringYamlTypeConverter())
        .Build();

    [Command("inspect")]
    public async Task Inspect(string path)
    {
        var yaml = await File.ReadAllTextAsync(path);
        var dto = _deserializer.Deserialize<JobTemplateManifestDto>(yaml);

        var template = dto.ToDomain();

        var tree = new Tree("Template");
        tree.AddNode($"Name: [purple_2]{template.Name.Value}[/]");
        tree.AddNode($"Author: [purple_2]{template.Author.Value}[/]");
        tree.AddNode($"Version: [purple_2]{template.Version.Value}[/]");
        tree.AddNode($"Display name: [purple_2]{template.DisplayName}[/]");
        tree.AddNode($"Description: [purple_2]{template.Description}[/]");

        var propertiesNode = tree.AddNode("Properties");

        var propertiesTable = new Table()
            .AddColumn("Name")
            .AddColumn("Display name")
            .AddColumn("Description")
            .AddColumn("Default value");

        foreach (var property in template.Properties)
            propertiesTable.AddRow(
                $"{property.Name}",
                $"{property.DisplayName}",
                $"{property.Description}",
                $"{property.DefaultValue}");

        propertiesNode.AddNode(propertiesTable);

        var inputsNode = tree.AddNode("Inputs");

        var inputsTable = new Table()
            .AddColumn("Name")
            .AddColumn("Display name")
            .AddColumn("Description");

        foreach (var input in template.Inputs)
            inputsTable.AddRow(
                $"{input.Name}",
                $"{input.DisplayName}",
                $"{input.Description}");

        inputsNode.AddNode(inputsTable);

        var outputsNode = tree.AddNode("Outputs");

        var outputsTable = new Table()
            .AddColumn("Name")
            .AddColumn("Display name")
            .AddColumn("Description");

        foreach (var output in template.Outputs)
            outputsTable.AddRow(
                $"{output.Name}",
                $"{output.DisplayName}",
                $"{output.Description}");

        outputsNode.AddNode(outputsTable);

        AnsiConsole.Write(tree);
        AnsiConsole.MarkupLine($"[green]Successfully retrieved job template: {template.Name}[/]");
    }
}