using AuctionData.Domain.Common.Models;

namespace AuctionData.Domain.Auction;

public sealed class Auction : Entity<long>
{
    public ItemListing ItemListing { get; private set; }
    public long Buyout { get; private set; }
    public long Quantity { get; private set; }
    public TimeLeft TimeLeft { get; private set; }
    public long? Bid { get; private set; }
    private Auction() : base(default) { }

    public Auction(long id,
                   ItemListing itemListing,
                   long buyout,
                   long quantity,
                   TimeLeft timeLeft,
                   long? bid = null) : base(id)
    {
        ItemListing = itemListing;
        Buyout = buyout;
        Quantity = quantity;
        TimeLeft = timeLeft;
        Bid = bid;
    }
}
