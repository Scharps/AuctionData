using System.ComponentModel.DataAnnotations.Schema;
using AuctionData.Domain.Common.Models;

namespace AuctionData.Domain.Auction;

public sealed class ItemListing : Entity<long>
{
    public long ItemId { get; private set; }
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
    public ICollection<Modifier> Modifiers { get; private set; }
    private ItemListing() : base(default) { }
    public ItemListing(long itemId,
                       long[] bonuses,
                       Modifier[] modifiers,
                       long id = default) : base(id)
    {
        ItemId = itemId;
        Bonuses = bonuses;
        Modifiers = modifiers;
    }
}
