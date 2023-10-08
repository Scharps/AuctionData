using System.Text.Json;
using System.Text.Json.Serialization;

namespace AuctionData.Application.Services.BlizzardApi.Auction;

internal sealed class TimeLeftConverter : JsonConverter<TimeLeftDto>
{
    public override TimeLeftDto Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();

        switch (value)
        {
            case "LONG":
                return TimeLeftDto.Long;
            case "VERY_LONG":
                return TimeLeftDto.VeryLong;
            case "MEDIUM":
                return TimeLeftDto.Medium;
            case "SHORT":
                return TimeLeftDto.Short;
        }
        throw new Exception($"Cannot unmarshal type TimeLeft of value: {value}.");
    }

    public override void Write(Utf8JsonWriter writer, TimeLeftDto value, JsonSerializerOptions options)
    {
        switch (value)
        {
            case TimeLeftDto.Long:
                JsonSerializer.Serialize(writer, "LONG", options);
                return;
            case TimeLeftDto.Medium:
                JsonSerializer.Serialize(writer, "MEDIUM", options);
                return;
            case TimeLeftDto.Short:
                JsonSerializer.Serialize(writer, "SHORT", options);
                return;
            case TimeLeftDto.VeryLong:
                JsonSerializer.Serialize(writer, "VERY_LONG", options);
                return;
        }
    }

    public static readonly TimeLeftConverter Singleton = new();
}