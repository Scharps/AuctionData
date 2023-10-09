using System.Text.Json;
using System.Web;
using AuctionData.Application.BlizzardApi.Item;
using AuctionData.Application.Entities.Auction;
using AuctionData.Application.Entities.Item;
using AuctionData.Application.Services.BlizzardApi.Auction;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Mvc;

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

    public async Task<IReadOnlyCollection<AuctionLog>> RequestConnectedRealmDataAsync(int connectedRealmId)
    {
        var now = DateTime.Now;

        var auctionData = await RequestDataAsync<AuctionDataDto>(
            NamespaceCategory.Dynamic,
            Region.EU,
            $"/data/wow/connected-realm/{connectedRealmId}/auctions");

        return auctionData.GetDomainAuctions()
            .Select(auc => new AuctionLog() { Auction = auc, RetrievedUtc = now })
            .ToArray();
    }

    public async Task<Item> GetItemDetailsAsync(long itemId)
    {
        var itemDataDto = await RequestDataAsync<ItemDataDto>(
            NamespaceCategory.Static,
            Region.EU,
            $"/data/wow/item/{itemId}");

        return itemDataDto.ToItem();
    }

    private async Task<TDto> RequestDataAsync<TDto>(NamespaceCategory namespaceCategory,
                                                    Region region,
                                                    string requestUri)
    {
        var accessToken = await _oAuthTokenManager.RequestToken();

        string @namespace = CreateNamespace(namespaceCategory, region);
        var query = HttpUtility.ParseQueryString(string.Empty);
        query["namespace"] = @namespace;
        query["access_token"] = accessToken.AccessToken;
        query["locale"] = "en_US";

        var uri = requestUri + $"?{query}";

        var responseMessage = await _httpClient.GetAsync(
            uri,
            HttpCompletionOption.ResponseHeadersRead
        );

        responseMessage.EnsureSuccessStatusCode();

        var dto = await responseMessage.Content.ReadFromJsonAsync<TDto>(_serializerOptions);

        if (dto is null) throw new InvalidOperationException("Deserialization of json content yeilded a null object.");

        return dto;
    }

    private static string CreateNamespace(NamespaceCategory namespaceCategory, Region region)
    {
        return $"{namespaceCategory.ToString().ToLower()}-{region.ToString().ToLower()}";
    }

    private enum NamespaceCategory
    {
        Static,
        Dynamic,
        Profile
    }

    private enum Region
    {
        US,
        EU,
        KR,
        TW
    }
}
