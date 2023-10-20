using System.Text.Json.Serialization;

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
    public TimeLeftDto TimeLeft { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("bid")]
    public long? Bid { get; }

    public AuctionDto(long id,
                   ItemDto item,
                   long buyout,
                   long quantity,
                   TimeLeftDto timeLeft,
                   long? bid = null)
    {
        Id = id;
        Item = item;
        Buyout = buyout;
        Quantity = quantity;
        TimeLeft = timeLeft;
        Bid = bid;
    }

    public Entities.Auction.Auction ToAuction(DateTime auctionReceivedAt)
    {
        return new()
        {
            Id = Id,
            Item = new Entities.Item.Item
            {
                Id = Item.Id
            },
            FirstSeen = auctionReceivedAt,
            LastSeen = auctionReceivedAt,
            Modifiers = Item.Modifiers.Select(m => new Entities.Auction.Modifier { Value = m.Value, Type = m.Type }).ToArray(),
            Bid = Bid,
            Buyout = Buyout,
            Quantity = Quantity,
            ExpectedExpiry = TimeLeft switch
            {
                TimeLeftDto.Long => auctionReceivedAt + TimeSpan.FromHours(12),
                TimeLeftDto.Medium => auctionReceivedAt + TimeSpan.FromHours(2),
                TimeLeftDto.Short => auctionReceivedAt + TimeSpan.FromMinutes(30),
                TimeLeftDto.VeryLong => auctionReceivedAt + TimeSpan.FromHours(48),
                _ => throw new NotSupportedException($"Cannot map {nameof(TimeLeft)}: {TimeLeft} to {nameof(Entities.Auction.Auction.ExpectedExpiry)}")
            }
        };
    }
}
