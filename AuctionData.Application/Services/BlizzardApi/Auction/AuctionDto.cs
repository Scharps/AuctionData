using System.Text.Json.Serialization;
using AuctionData.Application.Entities.Auction;

namespace AuctionData.Application.Services.BlizzardApi.Auction;

internal sealed class AuctionDto
{
    [JsonPropertyName("id")]
    public long Id { get; }

    [JsonPropertyName("item")]
    public ItemDto Item { get; }

    [JsonPropertyName("buyout")]
    public long Buyout { get; }

    [JsonPropertyName("quantity")]
    public long Quantity { get; }

    [JsonPropertyName("time_left")]
    public TimeLeft TimeLeft { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("bid")]
    public long? Bid { get; }

    public AuctionDto(long id,
                   ItemDto item,
                   long buyout,
                   long quantity,
                   TimeLeft timeLeft,
                   long? bid = null)
    {
        Id = id;
        Item = item;
        Buyout = buyout;
        Quantity = quantity;
        TimeLeft = timeLeft;
        Bid = bid;
    }

    public Entities.Auction.Auction ToAuction()
    {
        var itemListing = Item.ToItemListing();
        return new()
        {
            Id = Id,
            ItemListing = itemListing,
            Buyout = Buyout,
            Quantity = Quantity,
            TimeLeft = TimeLeft,
            Bid = Bid,
        };
    }
}
