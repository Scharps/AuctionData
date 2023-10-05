using System.Text.Json.Serialization;

namespace AuctionData.Infrastructure.BlizzardApi.Auction;

internal sealed class UrlDto
{
    [JsonPropertyName("href")]
    public Uri Href { get; }

    public UrlDto(Uri href)
    {
        Href = href;
    }
}
