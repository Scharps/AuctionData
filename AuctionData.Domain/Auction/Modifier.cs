using AuctionData.Domain.Common.Models;

namespace AuctionData.Domain.Auction;

public sealed class Modifier : Entity<long>
{
    public long Type { get; private set; }
    public long Value { get; private set; }

    private Modifier() : base(default) { }
    public Modifier(long type, long value, long id = default) : base(id)
    {
        Type = type;
        Value = value;
    }
}