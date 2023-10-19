using System.ComponentModel.DataAnnotations.Schema;

namespace AuctionData.Application.Entities.Auction;

public sealed class Auction : Entity
{
    public RegionAndRealmGroup ConnectedRealm { get; set; } = null!;
    public DateTime FirstSeen { get; set; }
    public DateTime? LastSeen { get; set; }
    public Item.Item Item { get; set; } = null!;
    public string InternalBonuses { get; private set; } = string.Empty;
    [NotMapped]
    public long[] Bonuses
    {
        get
        {
            return Array.ConvertAll(InternalBonuses.Split(';'), long.Parse);
        }
        set
        {
            InternalBonuses = string.Join(';', value ?? Array.Empty<long>());
        }
    }
    public ICollection<Modifier> Modifiers { get; set; } = null!;
    public long? Bid { get; set; }
    public long Buyout { get; set; }
    public long Quantity { get; set; }
    public DateTime ExpectedExpiry { get; set; }
}

public sealed class RegionAndRealmGroup : Entity
{
    public int GroupId { get; set; }
    public Region Region { get; set; }
}

public enum Region
{
    EU,
    US,
    TW,
}