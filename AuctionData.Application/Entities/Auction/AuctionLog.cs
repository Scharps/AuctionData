namespace AuctionData.Application.Entities.Auction;

public class AuctionLog : Entity
{
    public Auction Auction { get; set; } = null!;
    public DateTime RetrievedUtc { get; set; }
}