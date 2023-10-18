using System.Text.Json.Serialization;

namespace AuctionData.Application.Services.BlizzardApi.Auction;

internal sealed class ModifierDto
{

    [JsonPropertyName("type")]
    public long Type { get; }

    [JsonPropertyName("value")]
    public long Value { get; }
    public ModifierDto(long type, long value)
    {
        Type = type;
        Value = value;
    }
}
