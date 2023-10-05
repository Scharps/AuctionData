using System.Text.Json;
using System.Text.Json.Serialization;
using AuctionData.Domain.Auction;

namespace AuctionData.Infrastructure.BlizzardApi.Auction;

internal sealed class TimeLeftConverter : JsonConverter<TimeLeft>
{
    public override TimeLeft Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();

        switch (value)
        {
            case "LONG":
                return TimeLeft.Long;
            case "VERY_LONG":
                return TimeLeft.VeryLong;
            case "MEDIUM":
                return TimeLeft.Medium;
            case "SHORT":
                return TimeLeft.Short;
        }
        throw new Exception($"Cannot unmarshal type TimeLeft of value: {value}.");
    }

    public override void Write(Utf8JsonWriter writer, TimeLeft value, JsonSerializerOptions options)
    {
        switch (value)
        {
            case TimeLeft.Long:
                JsonSerializer.Serialize(writer, "LONG", options);
                return;
            case TimeLeft.Medium:
                JsonSerializer.Serialize(writer, "MEDIUM", options);
                return;
            case TimeLeft.Short:
                JsonSerializer.Serialize(writer, "SHORT", options);
                return;
            case TimeLeft.VeryLong:
                JsonSerializer.Serialize(writer, "VERY_LONG", options);
                return;
        }
    }

    public static readonly TimeLeftConverter Singleton = new();
}