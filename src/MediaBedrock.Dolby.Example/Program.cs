using MediaBedrock.Dolby.EncodingEngine;
using MediaBedrock.Dolby.Jobs.Models;
using MediaBedrock.Dolby.Jobs.Models.Filters;
using MediaBedrock.Dolby.Jobs.Models.Inputs;
using MediaBedrock.Dolby.Jobs.Models.Outputs;

const string path = "/path/to/dee";
var encodingEngine = new DolbyEncodingEngine(path, true);

var job = JobDefinition.CreateBuilder()
    .WithInput(
        Ec3Input.CreateBuilder()
            .WithFilePath("/path/to/input.ec3")
            .IsAtmos(true)
            .Build()
    )
    .WithFilter(
        DecodeDolbyDigitalPlus.CreateBuilder()
            .WithDownmixConfiguration(DolbyDigitalPlusDownMixConfiguration.Stereo)
            .Build()
    )
    .WithOutputs(
        WavOutput.CreateBuilder()
            .WithFilePath("/path/to/output.wav")
            .Build(),
        ManifestOutput.CreateBuilder()
            .WithFilePath("/path/to/manifest.xml")
            .Build()
    )
    .Build();

await encodingEngine.ProcessJobAsync(job);