using AuctionData.Application.Data;
using AuctionData.Application.Entities.Auction;
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

            await AddUnknownItemsAsync(auctionData, cancellationToken);

            await UpdateAndLinkItemsAsync(auctionData, cancellationToken);


            // await LinkAssociatedItemListings(auctionData, cancellationToken);

            _dbContext.Auctions.UpdateRange(auctionData);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        private async Task UpdateAndLinkItemsAsync(IReadOnlyCollection<Auction> auctionData, CancellationToken cancellationToken)
        {
            await UpdateItems(auctionData, cancellationToken);

            await LinkItemToAuction(auctionData, cancellationToken);

            _dbContext.UpdateRange(auctionData);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        private async Task UpdateItems(IReadOnlyCollection<Auction> auctionData, CancellationToken cancellationToken)
        {
            var auctionIdMap = auctionData.ToDictionary(auc => auc.Id, auc => auc);
            var presentAuctions = await _dbContext.Auctions
                .Where(auction => auctionIdMap.Keys.Contains(auction.Id))
                .ToListAsync(cancellationToken);

            foreach (var presentAuction in presentAuctions)
            {
                var newAuctionState = auctionIdMap[presentAuction.Id];

                newAuctionState.FirstSeen = presentAuction.FirstSeen;
                if (newAuctionState.ExpectedExpiry < presentAuction.ExpectedExpiry)
                {
                    newAuctionState.ExpectedExpiry = presentAuction.ExpectedExpiry;
                }
            }
        }

        private async Task LinkItemToAuction(IReadOnlyCollection<Auction> auctionData, CancellationToken cancellationToken)
        {
            var itemIds = new HashSet<long>(auctionData.Select(auc => auc.Item.Id));

            var items = await _dbContext.Items
                .Where(item => itemIds.Contains(item.Id))
                .ToListAsync(cancellationToken);

            var itemMap = items.ToDictionary(item => item.Id, item => item);

            foreach (var auction in auctionData)
            {
                auction.Item = itemMap[auction.Item.Id];
            }
        }

        // private async Task LinkAssociatedItemListings(IReadOnlyCollection<Auction> auctionData, CancellationToken cancellationToken)
        // {
        //     var auctionIds = auctionData.Select(log => log.Auction.Id).ToList();

        //     var existingAuctions = _dbContext.AuctionLogs
        //         .Where(log => auctionIds.Contains(log.Auction.Id))
        //         .Select(log => log.Auction)
        //         .GroupBy(auc => auc.Id)
        //         .Select(group => group.First());

        //     var existingAuctionsDictionary = await existingAuctions.ToDictionaryAsync(auc => auc.Id, auc => auc.ItemListing.Id, cancellationToken);


        //     foreach (var al in auctionData)
        //     {
        //         al.Auction.ItemListing.Id = existingAuctionsDictionary.GetValueOrDefault(al.Auction.Id);
        //     }
        // }

        private async Task AddUnknownItemsAsync(IReadOnlyCollection<Auction> auctionData, CancellationToken cancellationToken)
        {
            var itemIds = auctionData.Select(auc => auc.Item.Id).Distinct().ToList();

            var knownItemIds = await _dbContext.Items.Where(item => itemIds.Contains(item.Id))
                .Select(item => item.Id)
                .ToListAsync(cancellationToken);

            var unknownIds = itemIds.Except(knownItemIds);

            await foreach (var item in _client.GetItemsAsync(unknownIds, 5))
            {
                _dbContext.Items.Add(item);
            }
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public sealed class ProcessAuctionDataHostedService : IHostedService, IDisposable
    {
        private Timer? _timer = null!;
        private readonly IServiceProvider _serviceProvider;

        public ProcessAuctionDataHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(
                RequestAuctionDataProcessing,
                null,
                TimeSpan.Zero,
                TimeSpan.FromHours(1));

            return Task.CompletedTask;
        }

        private async void RequestAuctionDataProcessing(object? state)
        {
            await using var scope = _serviceProvider.CreateAsyncScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            await mediator.Send(new ProcessAuctionDataCommand(1305));
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}