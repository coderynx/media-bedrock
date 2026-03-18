namespace MediaBedrock.Controller.Presentation.Jobs.Contracts;

public sealed record JobParametersDto
{
    public required string TemplateName { get; init; }
    public Dictionary<string, string> Inputs { get; init; } = new();
    public Dictionary<string, string> Outputs { get; init; } = new();
    public Dictionary<string, string> Properties { get; init; } = new();
}