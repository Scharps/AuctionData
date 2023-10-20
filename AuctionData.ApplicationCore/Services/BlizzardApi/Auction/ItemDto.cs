using System.Text.Json.Serialization;
using AuctionData.Application.Entities.Auction;

namespace AuctionData.Application.Services.BlizzardApi.Auction;

internal sealed class ItemDto
{

    [JsonPropertyName("id")]
    public long Id { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("context")]
    public long? Context { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("bonus_lists")]
    public long[] BonusLists { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("modifiers")]
    public ModifierDto[] Modifiers { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("pet_breed_id")]
    public long? PetBreedId { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("pet_level")]
    public long? PetLevel { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("pet_quality_id")]
    public long? PetQualityId { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("pet_species_id")]
    public long? PetSpeciesId { get; }

    public ItemDto(long id,
                long? context,
                long[] bonusLists,
                ModifierDto[] modifiers,
                long? petBreedId,
                long? petLevel,
                long? petQualityId,
                long? petSpeciesId)
    {
        Id = id;
        Context = context;
        BonusLists = bonusLists;
        Modifiers = modifiers;
        PetBreedId = petBreedId;
        PetLevel = petLevel;
        PetQualityId = petQualityId;
        PetSpeciesId = petSpeciesId;
    }
}
