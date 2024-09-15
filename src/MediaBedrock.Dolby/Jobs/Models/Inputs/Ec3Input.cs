namespace MediaBedrock.Dolby.Jobs.Models.Inputs;

public sealed record Ec3Input : IJobInput
{
    public required string FilePath { get; init; }
    public required bool IsAtmos { get; init; }

    public static IEc3InputBuilderInitialStage CreateBuilder()
    {
        return new Ec3InputBuilder();
    }
}

public interface IEc3InputBuilderInitialStage
{
    IEc3InputBuilderFinalStage WithFilePath(string filePath);
}

public interface IEc3InputBuilderFinalStage
{
    IEc3InputBuilderFinalStage IsAtmos(bool isAtmos);
    Ec3Input Build();
}

public sealed class Ec3InputBuilder :
    IEc3InputBuilderInitialStage,
    IEc3InputBuilderFinalStage
{
    private string _filePath = null!;
    private bool _isAtmos;

    internal Ec3InputBuilder()
    {
    }

    public IEc3InputBuilderFinalStage IsAtmos(bool isAtmos)
    {
        _isAtmos = isAtmos;
        return this;
    }

    public Ec3Input Build()
    {
        return new Ec3Input
        {
            FilePath = _filePath,
            IsAtmos = _isAtmos
        };
    }

    public IEc3InputBuilderFinalStage WithFilePath(string filePath)
    {
        _filePath = filePath;
        return this;
    }
}