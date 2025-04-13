using System.Text.Json;
using System.Text.Json.Serialization;
using MediaBedrock.Dolby.Jobs.Dto;

namespace MediaBedrock.Dolby.Jobs.Serializers;

internal static class JobDefinitionDtoSerializer
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public static string Serialize(JobDefinitionDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        return JsonSerializer.Serialize(dto, JsonOptions);
    }

    public static JobDefinitionDto Deserialize(string json)
    {
        ArgumentNullException.ThrowIfNull(json);

        return JsonSerializer.Deserialize<JobDefinitionDto>(json, JsonOptions) ??
               throw new JsonException("Failed to deserialize JSON.");
    }
}