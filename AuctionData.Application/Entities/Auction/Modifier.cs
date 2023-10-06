using AuctionData.Domain.Common.Models;

namespace AuctionData.Application.Entities.Auction;

public sealed class Modifier : Entity
{
    public long Type { get; set; }
    public long Value { get; set; }
}