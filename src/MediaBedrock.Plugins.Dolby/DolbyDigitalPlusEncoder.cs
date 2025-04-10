using MediaBedrock.Dolby.EncodingEngine;
using MediaBedrock.Dolby.Jobs.Models;
using MediaBedrock.Dolby.Jobs.Models.Filters;
using MediaBedrock.Dolby.Jobs.Models.Inputs;
using MediaBedrock.Dolby.Jobs.Models.Outputs;
using MediaBedrock.Sdk.Processors;
using Microsoft.Extensions.Logging;

namespace MediaBedrock.Plugins.Dolby;

[Processor("dolby", "ddp-encoder")]
public sealed class DolbyDigitalPlusEncoder(
    ILogger<DolbyDigitalPlusEncoder> logger,
    ILoggerFactory loggerFactory)
    : IProcessor
{
    public async Task<ProcessorResult> ProcessAsync(ProcessorContext context, CancellationToken ct = default)
    {
        var inputTrack = context.GetInput("input");
        if (inputTrack is null)
        {
            logger.LogError("Input track not found");
            return ProcessorResult.Failure("Input track not found");
        }

        var outputTrack = context.GetOutput("output");
        if (outputTrack is null)
        {
            logger.LogError("Output track not found");
            return ProcessorResult.Failure("Output track not found");
        }

        var inputUri = inputTrack.GetAsFilePath();
        var outputUri = outputTrack.GetAsFilePath();

        logger.LogInformation("Encoding {Input} to {Output}", inputUri, outputUri);

        var job = JobDefinition.CreateBuilder();

        if (inputTrack.MediaInformation.Format.Equals("Wave"))
        {
            job.WithInput(
                AtmosMezzanineInput.CreateBuilder()
                    .WithFilePath(inputUri)
                    .Build());
        }
        else
        {
            return ProcessorResult.Failure("Input track must be an Atmos mezzanine");
        }

        var lineDrc = context.GetPropertyRequired("LineDrcProfile")
            .Transform(input => Enum.TryParse<DrcProfile>(input, out var profile) ? profile : DrcProfile.MusicStandard);

        var rightLeftDrc = context.GetPropertyRequired("RightLeftDrcProfile")
            .Transform(input => Enum.TryParse<DrcProfile>(input, out var profile) ? profile : DrcProfile.MusicStandard);

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

        var enginePath = context.GetProperty("EnginePath")?.GetValue();
        if (string.IsNullOrEmpty(enginePath))
        {
            return ProcessorResult.Failure("Engine path is not set");
        }

        var useWine = context.GetPropertyRequired("UseWine")
            .GetValue("true")
            .Equals("true", StringComparison.OrdinalIgnoreCase);

        var engine = new DolbyEncodingEngine(
            path: enginePath,
            useWine: useWine,
            loggerFactory: loggerFactory);

        await engine.ProcessJobAsync(jobDefinition);

        logger.LogInformation("Successfully encoded {Input} to {Output}", inputUri, outputUri);

        return ProcessorResult.Success();
    }
}