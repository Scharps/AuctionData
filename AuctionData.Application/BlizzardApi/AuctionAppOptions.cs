namespace AuctionData.Application.BlizzardApi;

public sealed class AuctionAppOptions
{
    public AuctionAppOptions(string clientId, string clientSecret)
    {
        ClientId = clientId;
        ClientSecret = clientSecret;
    }

    public string ClientId { get; }
    public string ClientSecret { get; }
}