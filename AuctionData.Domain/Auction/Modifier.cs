using AuctionData.Domain.Common.Models;

namespace AuctionData.Domain.Auction;

public sealed class Modifier : Entity
{
    public long Type { get; private set; }
    public long Value { get; private set; }

}