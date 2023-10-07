using System.Text.Json;
using System.Web;
using AuctionData.Application.BlizzardApi.Auction;
using AuctionData.Application.Entities.Auction;

namespace AuctionData.Application.BlizzardApi;
public class Client
{
    private readonly static JsonSerializerOptions _serializerOptions = new(JsonSerializerDefaults.General)
    {
        Converters =
        {
            TimeLeftConverter.Singleton
        }
    };
    private readonly HttpClient _httpClient;
    private readonly OAuthTokenManager _oAuthTokenManager;

    public Client(HttpClient httpClient, OAuthTokenManager oAuthTokenManager)
    {
        if (httpClient.BaseAddress == null)
            throw new Exception("Base address of the HttpClient must be configured");
        _httpClient = httpClient;
        _oAuthTokenManager = oAuthTokenManager;
    }

    public async Task<IReadOnlyCollection<Entities.Auction.AuctionLog>> RequestConnectedRealmData(int connectedRealmId)
    {
        // Ensure token validity.
        var accessToken = await _oAuthTokenManager.RequestToken();

        var query = HttpUtility.ParseQueryString(string.Empty);
        query["namespace"] = "dynamic-eu";
        query["access_token"] = accessToken.AccessToken;
        var queryString = query.ToString();

        // HttpCompletionOption used to avoid timeout on content
        var responseMessage = await _httpClient.GetAsync(
            $"/data/wow/connected-realm/{connectedRealmId}/auctions?{queryString}",
            HttpCompletionOption.ResponseHeadersRead);

        responseMessage.EnsureSuccessStatusCode();

        var now = DateTime.UtcNow;

        var auctionData = await responseMessage.Content.ReadFromJsonAsync<AuctionDataDto>(_serializerOptions);

        if (auctionData is null) throw new Exception($"Deserialized {nameof(auctionData)} is null.");

        return auctionData.GetDomainAuctions()
            .Select(auc => new AuctionLog() { Auction = auc, RetrievedUtc = now })
            .ToArray();
    }
}
