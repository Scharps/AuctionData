using System.Text.Json.Serialization;

namespace AuctionData.Application.Services.BlizzardApi.Common;

internal sealed class LinksDto
{
    [JsonPropertyName("self")]
    public UrlDto Self { get; }

    public LinksDto(UrlDto self)
    {
        Self = self;
    }
}
