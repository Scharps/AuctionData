using System.Threading.RateLimiting;

namespace AuctionData.Application.Services.BlizzardApi;
public sealed class BlizzardAppOptions
{
    public required string ClientId { get; init; }
    public required string ClientSecret { get; init; }
    public required int TokenLimit { get; init; }
    public required string QueueProcessingOrder { get; init; }
    public required int QueueLimit { get; init; }
    public required int ReplenishmentPeriod { get; init; }
    public required int TokensPerPeriod { get; init; }
    public required bool AutoReplenishment { get; init; }
}