using MediaBedrock.Dolby.Jobs.Dto;
using MediaBedrock.Dolby.Jobs.Models.Filters;
using MediaBedrock.Dolby.Jobs.Models.Inputs;
using MediaBedrock.Dolby.Jobs.Models.Outputs;

namespace MediaBedrock.Dolby.Jobs.Models;

internal static class JobDefinitionExtensions
{
    internal static JobDefinitionDto ToDto(this JobDefinition definition)
    {
        return new JobDefinitionDto
        {
            JobConfig = new JobConfigDto
            {
                Input = definition.Input.ToDto(),
                Filter = definition.Filter.ToDto(),
                Output = definition.Outputs.ToDto(),
                Misc = null!
            }
        };
    }

    private static JobInputDto ToDto(this IJobInput input)
    {
        return input switch
        {
            AudioDefinitionModelInput audioDefinitionModel => AudioDefinitionModelInputExtensions.ToDto(
                audioDefinitionModel),
            AtmosMezzanineInput atmosMezzanine => AtmosMezzanineInputExtensions.ToDto(atmosMezzanine),
            Ec3Input ec3 => Ec3InputExtensions.ToDto(ec3),
            _ => throw new ArgumentOutOfRangeException(nameof(input))
        };
    }

    private static JobFilterDto ToDto(this IJobFilter filter)
    {
        return filter switch
        {
            EncodeToAtmosDolbyDigitalPlus atmosDolbyDigitalPlus => EncodeToAtmosDolbyDigitalPlusExtensions.ToDto(
                atmosDolbyDigitalPlus),
            EncodeToImsAc4 imsAc4 => EncodeToImsAc4Extensions.ToDto(imsAc4),
            EncodeToDolbyTrueHd trueHd => EncodeDolbyTrueHdExtensions.ToDto(trueHd),
            DecodeDolbyDigitalPlus dolbyDigitalPlus => DecodeDolbyDigitalPlusExtensions.ToDto(dolbyDigitalPlus),
            _ => throw new ArgumentOutOfRangeException(nameof(filter))
        };
    }

    private static JobOutputDto ToDto(this IJobOutput[] outputs)
    {
        return new JobOutputDto
        {
            Mp4 = outputs.OfType<Mp4Output>()
                .Select(Mp4OutputExtensions.ToDto)
                .FirstOrDefault(),

            Mlp = outputs.OfType<MlpOutput>()
                .Select(MlpOutputExtensions.ToDto)
                .FirstOrDefault(),

            Wav = outputs.OfType<WavOutput>()
                .Select(WavOutputExtensions.ToDto)
                .FirstOrDefault(),

            Manifest = outputs.OfType<ManifestOutput>()
                .Select(ManifestOutputExtensions.ToDto)
                .FirstOrDefault()
        };
    }
}