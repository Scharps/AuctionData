using System.Text.Json.Serialization;

namespace AuctionData.Infrastructure.BlizzardApi.Auction;

internal sealed class LinksDto
{
    [JsonPropertyName("self")]
    public UrlDto Self { get; }

    public LinksDto(UrlDto self)
    {
        Self = self;
    }
}
