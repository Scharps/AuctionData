using System.Text.Json.Serialization;
using AuctionData.Application.Services.BlizzardApi.Common;

namespace AuctionData.Application.BlizzardApi.Item;

internal class ItemDataDto
{
    [JsonPropertyName("_links")]
    public LinksDto Links { get; }

    [JsonPropertyName("id")]
    public long Id { get; }

    [JsonPropertyName("name")]
    public string Name { get; }

    public ItemDataDto(LinksDto links, long id, string name)
    {
        Links = links;
        Id = id;
        Name = name;
    }

    internal Entities.Item.Item ToItem()
    {
        return new()
        {
            Id = Id,
            Name = Name
        };
    }

    #region Ommitted Properties

    // [JsonPropertyName("quality")]
    // public InventoryType Quality { get; set; }

    // [JsonPropertyName("level")]
    // public long Level { get; set; }

    // [JsonPropertyName("required_level")]
    // public long RequiredLevel { get; set; }

    // [JsonPropertyName("media")]
    // public Media Media { get; set; }

    // [JsonPropertyName("item_class")]
    // public ItemClass ItemClass { get; set; }

    // [JsonPropertyName("item_subclass")]
    // public ItemClass ItemSubclass { get; set; }

    // [JsonPropertyName("inventory_type")]
    // public InventoryType InventoryType { get; set; }

    // [JsonPropertyName("purchase_price")]
    // public long PurchasePrice { get; set; }

    // [JsonPropertyName("sell_price")]
    // public long SellPrice { get; set; }

    // [JsonPropertyName("max_count")]
    // public long MaxCount { get; set; }

    // [JsonPropertyName("is_equippable")]
    // public bool IsEquippable { get; set; }

    // [JsonPropertyName("is_stackable")]
    // public bool IsStackable { get; set; }

    // [JsonPropertyName("description")]
    // public string Description { get; set; }

    // [JsonPropertyName("preview_item")]
    // public PreviewItem PreviewItem { get; set; }

    // [JsonPropertyName("purchase_quantity")]
    // public long PurchaseQuantity { get; set; }

    #endregion
}