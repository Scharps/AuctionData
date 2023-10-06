using System.ComponentModel.DataAnnotations.Schema;

namespace AuctionData.Application.Entities.Auction;

public sealed class ItemListing
{
    public long ItemId { get; set; }
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
}
