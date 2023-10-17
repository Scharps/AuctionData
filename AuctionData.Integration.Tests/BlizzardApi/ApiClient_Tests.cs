using AuctionData.Application.Entities.Item;
using AuctionData.Application.Services.BlizzardApi;
using Xunit.Abstractions;
using Xunit.Microsoft.DependencyInjection.Abstracts;

namespace AuctionData.Integration.Tests.BlizzardApi;

public class ApiClient_Tests : TestBed<ApiClientFixture>
{
    public ApiClient_Tests(ITestOutputHelper testOutputHelper, ApiClientFixture fixture) : base(testOutputHelper, fixture)
    {
    }

    [Theory]
    [MemberData(nameof(GetItemsAsync_Test_Data))]
    public async Task GetItemsAsync_Test(params int[] itemIds)
    {
        var client = _fixture.GetService<Client>(_testOutputHelper);
        if (client is null) throw new NullReferenceException(nameof(client));

        var items = new List<Item>();

        await foreach (var item in client.GetItemsAsync(itemIds.Select(n => (long)n), 5))
        {
            items.Add(item);
        }

        items.Count.Should().Be(4);
    }


    public static IEnumerable<object[]> GetItemsAsync_Test_Data()
    {
        var items = new List<object[]> {
            new object[] { 14200, 19019, 204177, 202606 }
        };

        return items;
    }
}
