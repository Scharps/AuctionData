using System.Text.Json.Serialization;

namespace AuctionData.Application.BlizzardApi.Auction;

internal sealed class LinksDto
{
    [JsonPropertyName("self")]
    public UrlDto Self { get; }

    public LinksDto(UrlDto self)
    {
        Self = self;
    }
}
