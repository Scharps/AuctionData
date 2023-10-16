using System.Text.Json.Serialization;

namespace AuctionData.Application.Services.BlizzardApi.Common;

internal sealed class UrlDto
{
    [JsonPropertyName("href")]
    public Uri Href { get; }

    public UrlDto(Uri href)
    {
        Href = href;
    }
}
