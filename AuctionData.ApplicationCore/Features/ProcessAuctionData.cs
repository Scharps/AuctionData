using System.Diagnostics;
using AuctionData.Application.Data;
using AuctionData.Application.Entities.Auction;
using AuctionData.Application.Entities.Item;
using AuctionData.Application.Services.BlizzardApi;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AuctionData.Application.Features;

public static class ProcessAuctionData
{
    public record ProcessAuctionDataCommand(int ConnectedRealmId) : IRequest;

    public sealed class ProcessAuctionDataHandler : IRequestHandler<ProcessAuctionDataCommand>
    {
        private readonly Client _client;
        private readonly AuctionDbContext _dbContext;

        public ProcessAuctionDataHandler(Client client, AuctionDbContext dbContext)
        {
            _client = client;
            _dbContext = dbContext;
        }

        public async Task Handle(ProcessAuctionDataCommand request, CancellationToken cancellationToken)
        {
            var auctionData = await _client.RequestConnectedRealmDataAsync(request.ConnectedRealmId);

            var itemIds = auctionData.Select(auc => auc.Auction.ItemListing.ItemId).Distinct().ToList();

            var knownItemIds = await _dbContext.Items.Where(item => itemIds.Contains(item.Id)).Select(item => item.Id).ToListAsync();

            var unknownIds = itemIds.Except(knownItemIds);

            await foreach (var item in _client.GetItemsAsync(unknownIds, 5))
            {
                _dbContext.Items.Add(item);
            }
            await _dbContext.SaveChangesAsync(cancellationToken);


            foreach (var auction in auctionData)
            {
                var t1 = FindAssociatedItemListingIfExists(auction, cancellationToken);

                var t2 = RequestItemDetailsIfUnknown(auction.Auction.ItemListing.ItemId, cancellationToken);

                await Task.WhenAll(t1, t2);
            }

            _dbContext.AddRange(auctionData, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Requests the item details from the Blizzard API if the <see cref="Item"/> is not contained within the DbContext and adds the it.  
        /// </summary>
        /// <param name="auction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task RequestItemDetailsIfUnknown(long itemId, CancellationToken cancellationToken)
        {
            if (!await _dbContext.Items.AnyAsync(item => item.Id == itemId, cancellationToken))
            {
                var item = await _client.GetItemAsync(itemId);
                _dbContext.Items.Add(item);
            }
        }

        /// <summary>
        /// Finds an auction with the same ID within the database and assigns the <see cref="Auction.ItemListing"/> within <paramref name="auction"/> if one exists.
        /// </summary>
        /// <param name="auction">The auction to search for.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>A task representing this process.</returns>
        private async Task FindAssociatedItemListingIfExists(AuctionLog auction, CancellationToken cancellationToken)
        {
            var itemListing = await _dbContext.AuctionLogs.Where(al => al.Auction.Id == auction.Id)
                .Select(al => al.Auction.ItemListing)
                .SingleOrDefaultAsync(cancellationToken);
            if (itemListing is not null)
            {
                auction.Auction.ItemListing = itemListing;
            }
        }
    }
}