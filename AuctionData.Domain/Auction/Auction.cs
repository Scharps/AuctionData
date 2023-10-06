using AuctionData.Domain.Common.Models;

namespace AuctionData.Domain.Auction;

public sealed class Auction : Entity
{
    public ItemListing ItemListing { get; set; } = null!;
    public long Buyout { get; set; }
    public long Quantity { get; set; }
    public TimeLeft TimeLeft { get; set; }
    public long? Bid { get; set; }
}
