using System.Text.Json;
using System.Web;
using AuctionData.Application.BlizzardApi.Item;
using AuctionData.Application.Entities.Auction;
using AuctionData.Application.Entities.Item;
using AuctionData.Application.Services.BlizzardApi.Auction;

namespace AuctionData.Application.Services.BlizzardApi;
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

    public async Task<IReadOnlyCollection<AuctionLog>> RequestConnectedRealmData(int connectedRealmId)
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

    public async Task<Item> GetItemDetails(long itemId)
    {
        var accessToken = await _oAuthTokenManager.RequestToken();

        var query = HttpUtility.ParseQueryString(string.Empty);
        query["namespace"] = "static-eu";
        query["access_token"] = accessToken.AccessToken;
        query["locale"] = "en_US";
        var queryString = query.ToString();

        var responseMessage = await _httpClient.GetAsync(
            $"/data/wow/item/{itemId}?{queryString}",
            HttpCompletionOption.ResponseHeadersRead);

        responseMessage.EnsureSuccessStatusCode();

        var itemDataDto = await responseMessage.Content.ReadFromJsonAsync<ItemDataDto>(_serializerOptions);

        if (itemDataDto is null) throw new Exception($"Deserialized {nameof(itemDataDto)} is null");

        return itemDataDto.ToItem();
    }
}
