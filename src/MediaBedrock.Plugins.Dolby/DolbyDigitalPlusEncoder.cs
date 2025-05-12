using System.Text;
using MediaBedrock.Dolby.Encoding;
using MediaBedrock.Dolby.Encoding.Messages;
using MediaBedrock.Dolby.Jobs.Models;
using MediaBedrock.Dolby.Jobs.Models.Filters;
using MediaBedrock.Dolby.Jobs.Models.Inputs;
using MediaBedrock.Dolby.Jobs.Models.Outputs;
using MediaBedrock.Sdk.Processors;
using Microsoft.Extensions.Logging;

namespace MediaBedrock.Plugins.Dolby;

[Processor("dolby", "ddp_encoder")]
public sealed class DolbyDigitalPlusEncoder : IProcessor
{
    public async Task<ProcessorResult> ProcessAsync(
        ProcessorContext context,
        CancellationToken cancellationToken = default)
    {
        const string inputKey = "input";
        const string outputKey = "output";
        const string lineDrcProfileKey = "line_drc_profile";
        const string rightLeftDrcKey = "right_left_drc_profile";
        const string enginePathKey = "engine_path";
        const string useWineKey = "use_wine";

        var inputTrack = context.GetInput(inputKey);
        if (inputTrack is null)
        {
            return ProcessorResult.Failure($"{inputKey} track not found");
        }

        var outputTrack = context.GetOutput(outputKey);
        if (outputTrack is null)
        {
            return ProcessorResult.Failure($"{outputKey} track not found");
        }

        var inputUri = inputTrack.GetAsFilePath();
        var outputUri = outputTrack.GetAsFilePath();

        context.Logger.LogInformation("Encoding {Input} to {Output}", inputUri, outputUri);

        var job = JobDefinition.CreateBuilder();

        if (inputTrack.MediaInformation.Format.Equals("Wave"))
        {
            if (IsBwf(inputUri))
            {
                job.WithInput(
                    AtmosMezzanineInput.CreateBuilder()
                        .WithFilePath(inputUri)
                        .Build());
            }
            else
            {
                job.WithInput(
                    WavInput.CreateBuilder()
                        .WithFilePath(inputUri)
                        .Build());
            }
        }
        else
        {
            return ProcessorResult.Failure("Input track must be an Atmos mezzanine");
        }

        var lineDrc = context.GetPropertyRequired(lineDrcProfileKey)
            .GetValue(input => Enum.TryParse<DrcProfile>(input, out var profile) ? profile : DrcProfile.MusicStandard);

        var rightLeftDrc = context.GetPropertyRequired(rightLeftDrcKey)
            .GetValue(input => Enum.TryParse<DrcProfile>(input, out var profile) ? profile : DrcProfile.MusicStandard);

        job.WithFilter(
            EncodeToAtmosDolbyDigitalPlus.CreateBuilder()
                .WithLineModeDrcProfile(lineDrc)
                .WithRfModeDrcProfile(rightLeftDrc)
                .Build());

        job.WithOutput(
            Mp4Output.CreateBuilder()
                .WithFilePath(outputUri)
                .Build());

        var jobDefinition = job.Build();

        var enginePath = context.GetProperty(enginePathKey)?.GetValue();
        if (string.IsNullOrEmpty(enginePath))
        {
            return ProcessorResult.Failure($"{enginePath} is not set");
        }

        var useWine = context.GetPropertyRequired(useWineKey)
            .GetValue("true")?
            .Equals("true", StringComparison.OrdinalIgnoreCase) ?? false;

        try
        {
            var engine = new DolbyEncodingEngine(enginePath, useWine);

            await engine.ProcessJobAsync(jobDefinition, OnStatusChange);
        }
        catch (Exception exception)
        {
            context.Logger.LogError(exception, "Failed to process job");
            return ProcessorResult.Failure("Failed to process job", exception);
        }

        context.Logger.LogInformation("Successfully encoded {Input} to {Output}", inputUri, outputUri);

        return ProcessorResult.Success();

        void OnStatusChange(DolbyEncodingEngineMessage message)
        {
            switch (message)
            {
                case DolbyEncodingEngineErrorMessage errorMessage:
                    context.Logger.LogError("Received error: {Error}", errorMessage.Message);
                    break;

                case DolbyEncodingEngineProgressMessage progressMessage:
                    context.Logger.LogInformation(
                        "Stage: {Stage}, Step: {Step}, Stage Progress: {StageProgress}, Overall Progress: {OverallProgress}",
                        progressMessage.Stage,
                        progressMessage.Step,
                        progressMessage.StageProgress,
                        progressMessage.OverallProgress);

                    break;

                default:
                    context.Logger.LogInformation("Received message {Message}", message.ToString());
                    break;
            }
        }
    }

    private static bool IsBwf(string filePath)
    {
        // TODO: This should be handled by the MediaInfoRetriever.

        using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        using var reader = new BinaryReader(stream);

        var riffHeader = Encoding.ASCII.GetString(reader.ReadBytes(4));
        if (riffHeader.Equals("RIFF"))
        {
            return false;
        }

        reader.BaseStream.Seek(4, SeekOrigin.Current);
        var waveHeader = Encoding.ASCII.GetString(reader.ReadBytes(4));
        if (waveHeader.Equals("WAVE"))
        {
            return false;
        }

        while (reader.BaseStream.Position < reader.BaseStream.Length)
        {
            var chunkId = Encoding.ASCII.GetString(reader.ReadBytes(4));
            var chunkSize = reader.ReadInt32();

            if (chunkId.Equals("bext"))
            {
                return true;
            }

            reader.BaseStream.Seek(chunkSize, SeekOrigin.Current);
        }

        return false;
    }
}