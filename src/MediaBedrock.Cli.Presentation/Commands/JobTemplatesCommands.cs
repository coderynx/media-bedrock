using Cocona;
using MediaBedrock.Cli.Domain.Jobs.Templates;
using MediaBedrock.Cli.Domain.Jobs.Templates.Interfaces;
using Spectre.Console;
using Spectre.Console.Json;

namespace MediaBedrock.Cli.Presentation.Commands;

public sealed class JobTemplatesCommands(
    IJobTemplateSerializerProvider serializerProvider,
    IJobTemplatesRepository repository)
{
    [Command("add")]
    public async Task AddAsync(string path)
    {
        var templateJson = await File.ReadAllTextAsync(path);

        var resolveSerializer = serializerProvider.ResolveSerializer(Path.GetExtension(path));
        if (resolveSerializer.IsFailure)
        {
            AnsiConsole.MarkupLine($"[red]Failed to resolve serializer: {resolveSerializer.Error.Message}[/]");
            return;
        }

        var createJobTemplate = resolveSerializer.Value.Deserialize(templateJson);
        if (createJobTemplate.IsFailure)
        {
            AnsiConsole.MarkupLine($"[red]Failed to deserialize job template: {createJobTemplate.Error.Message}[/]");
            return;
        }

        var template = createJobTemplate.Value;

        var storeTemplate = await repository.StoreAsync(template);
        if (storeTemplate.IsFailure)
        {
            AnsiConsole.MarkupLine($"[red]Failed to store job template: {storeTemplate.Error.Message}[/]");
            return;
        }

        AnsiConsole.MarkupLine($"[green]Successfully stored job template: {template.Name}[/]");
    }

    [Command("rm")]
    public async Task RemoveAsync(string path)
    {
        var templateJson = await File.ReadAllTextAsync(path);

        var resolveSerializer = serializerProvider.ResolveSerializer(Path.GetExtension(path));
        if (resolveSerializer.IsFailure)
        {
            AnsiConsole.MarkupLine($"[red]Failed to resolve serializer: {resolveSerializer.Error.Message}[/]");
            return;
        }

        var createJobTemplate = resolveSerializer.Value.Deserialize(templateJson);
        if (createJobTemplate.IsFailure)
        {
            AnsiConsole.MarkupLine($"[red]Failed to deserialize job template: {createJobTemplate.Error.Message}[/]");
            return;
        }

        var template = createJobTemplate.Value;

        var removeTemplate = await repository.DeleteAsync(template.Name);
        if (removeTemplate.IsFailure)
        {
            AnsiConsole.MarkupLine($"[red]Failed to remove job template: {removeTemplate.Error.Message}[/]");
            return;
        }

        AnsiConsole.MarkupLine($"[green]Successfully removed job template: {template.Name}[/]");
    }

    [Command("inspect")]
    public async Task InspectAsync(string name)
    {
        var createJobTemplate = JobTemplateName.Create(name);
        if (createJobTemplate.IsFailure)
        {
            AnsiConsole.MarkupLine($"[red]Failed to create job template name: {createJobTemplate.Error.Message}[/]");
            return;
        }

        var getTemplate = await repository.GetAsync(createJobTemplate.Value);
        if (!getTemplate.IsSome)
        {
            AnsiConsole.MarkupLine($"[red]Failed to get job template: {name}[/]");
            return;
        }

        var template = getTemplate.ValueOrThrow();

        var resolveSerializer = serializerProvider.ResolveSerializer(JobTemplateSerializerFormat.Json);
        if (resolveSerializer.IsFailure)
        {
            AnsiConsole.MarkupLine($"[red]Failed to resolve serializer: {resolveSerializer.Error.Message}[/]");
            return;
        }

        var serialize = resolveSerializer.Value.Serialize(template);
        if (serialize.IsFailure)
        {
            AnsiConsole.MarkupLine($"[red]Failed to serialize job template: {serialize.Error.Message}[/]");
            return;
        }

        var jsonText = new JsonText(serialize.Value);

        AnsiConsole.Write(
            new Panel(jsonText)
                .Header(template.Name.Value)
                .Collapse()
                .RoundedBorder()
                .BorderColor(Color.Yellow));

        AnsiConsole.MarkupLine($"[green]Successfully retrieved job template: {template.Name}[/]");
    }
}