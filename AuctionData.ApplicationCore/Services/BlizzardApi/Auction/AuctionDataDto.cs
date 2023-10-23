using System.Text.Json.Serialization;
using AuctionData.Application.Services.BlizzardApi.Common;

namespace AuctionData.Application.Services.BlizzardApi.Auction;

internal sealed class AuctionDataDto
{
    [JsonPropertyName("_links")]
    public LinksDto Links { get; }

    [JsonPropertyName("connected_realm")]
    public UrlDto ConnectedRealm { get; }

    [JsonPropertyName("auctions")]
    public AuctionDto[] Auctions { get; }

    [JsonPropertyName("commodities")]
    public UrlDto Commodities { get; }

    public AuctionDataDto(LinksDto links,
                          UrlDto connectedRealm,
                          AuctionDto[] auctions,
                          UrlDto commodities)
    {
        Links = links;
        ConnectedRealm = connectedRealm;
        Auctions = auctions;
        Commodities = commodities;
    }

    public IEnumerable<Entities.Auction.Auction> GetDomainAuctions(DateTime receivedAt)
    {
        return Auctions.Select(a => a.ToAuction(receivedAt));
    }
}
