using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Web;
using AuctionData.Application.BlizzardApi.Item;
using AuctionData.Application.Entities.Auction;
using AuctionData.Application.Entities.Item;
using AuctionData.Application.Services.BlizzardApi.Auction;
using Microsoft.VisualBasic;
using System;
using Microsoft.AspNetCore.Http.Features;
using System.Net;

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
    private readonly ILogger<Client> testLogger;

    public Client(HttpClient httpClient, ILogger<Client> testLogger)
    {
        if (httpClient.BaseAddress == null)
            throw new Exception("Base address of the HttpClient must be configured");
        _httpClient = httpClient;
        this.testLogger = testLogger;
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

    public async Task<Item> GetItemAsync(long itemId)
    {
        var itemDataDto = await RequestDataAsync<ItemDataDto>(
            NamespaceCategory.Static,
            Region.EU,
            $"/data/wow/item/{itemId}");

        return itemDataDto.ToItem();
    }

    public async IAsyncEnumerable<Item> GetItemsAsync(IEnumerable<long> itemIds, int maxDegreeOfParallelism)
    {
        if (!itemIds.Any()) yield break;

        var itemQueue = new ConcurrentQueue<long>(itemIds);
        var tasks = new List<Task<Item>>();
        var throttler = new SemaphoreSlim(maxDegreeOfParallelism);

        while (itemQueue.TryDequeue(out var itemId))
        {
            tasks.Add(WaitAndFetchItem(itemId));

            while (tasks.Any(t => t.Status == TaskStatus.RanToCompletion) || (tasks.Count != 0 && itemQueue.IsEmpty))
            {
                Task<Item>? completedTask = null;

                completedTask = await Task.WhenAny(tasks);
                tasks.Remove(completedTask);

                if (completedTask is not null && completedTask.Status == TaskStatus.RanToCompletion)
                {
                    yield return await completedTask;
                }
            }
        }

        async Task<Item> WaitAndFetchItem(long itemId)
        {
            await throttler!.WaitAsync();
            try
            {
                return await GetItemAsync(itemId);
            }
            catch (HttpRequestException e)
            {
                // The Blizzard item API has some missing items. These will be ignored for now #19
                if (e.StatusCode != HttpStatusCode.NotFound)
                {
                    itemQueue!.Enqueue(itemId);
                }
                throw;
            }
            finally
            {
                throttler!.Release();
            }
        }
    }

    private async Task<TDto> RequestDataAsync<TDto>(NamespaceCategory namespaceCategory,
                                                    Region region,
                                                    string requestUri)
    {
        HttpResponseMessage responseMessage = await MakeRequest(namespaceCategory, region, requestUri);

        return await DeserializeToDto<TDto>(responseMessage);
    }

    private async Task<TDto> DeserializeToDto<TDto>(HttpResponseMessage responseMessage)
    {
        var dto = await responseMessage.Content.ReadFromJsonAsync<TDto>(_serializerOptions);

        if (dto is null) throw new InvalidOperationException("Deserialization of json content yeilded a null object.");

        return dto;
    }

    private async Task<HttpResponseMessage> MakeRequest(NamespaceCategory namespaceCategory, Region region, string requestUri)
    {
        string @namespace = CreateNamespace(namespaceCategory, region);

        string uri = BuildUri(requestUri, @namespace);

        var responseMessage = await _httpClient.GetAsync(
            uri,
            HttpCompletionOption.ResponseHeadersRead
        );

        responseMessage.EnsureSuccessStatusCode();
        return responseMessage;
    }

    private static string BuildUri(string requestUri, string @namespace)
    {
        var query = HttpUtility.ParseQueryString(string.Empty);
        query["namespace"] = @namespace;
        query["locale"] = "en_US";

        var uri = requestUri + $"?{query}";
        return uri;
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
