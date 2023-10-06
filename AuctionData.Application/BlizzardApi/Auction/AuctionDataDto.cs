using System.Text.Json.Serialization;

namespace AuctionData.Application.BlizzardApi.Auction;

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

    public Entities.Auction.Auction[] GetDomainAuctions()
    {
        return Auctions.Select(a => a.ToAuction()).ToArray();
    }
}
