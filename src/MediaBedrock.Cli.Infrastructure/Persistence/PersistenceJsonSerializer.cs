using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MediaBedrock.Cli.Infrastructure.Persistence;

public static class PersistenceJsonSerializer
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = false,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        ReferenceHandler = ReferenceHandler.Preserve,
        Converters =
        {
            new JsonStringEnumConverter()
        }
    };

    public static string Serialize<TValue>(TValue @object)
    {
        return JsonSerializer.Serialize(@object, SerializerOptions);
    }

    public static TValue Deserialize<TValue>(string json)
    {
        return JsonSerializer.Deserialize<TValue>(json, SerializerOptions)
               ?? throw new SerializationException("Failed to deserialize JSON");
    }
}