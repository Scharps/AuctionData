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
        }

        private async Task UpdateAndLinkItemsAsync(IReadOnlyCollection<Auction> auctionData, CancellationToken cancellationToken)
        {
            await LinkItemToAuction(auctionData, cancellationToken);

            await UpdateAndAddAuctions(auctionData, cancellationToken);
        }

        private async Task UpdateAndAddAuctions(IReadOnlyCollection<Auction> auctionData, CancellationToken cancellationToken)
        {
            var auctionIdMap = auctionData.ToDictionary(auc => auc.Id, auc => auc);

            var auctionsToUpdate = await _dbContext.Auctions
                .Where(auction => auctionIdMap.Keys.Contains(auction.Id))
                .ToListAsync(cancellationToken);

            var auctionsToAdd = auctionData.Except(auctionsToUpdate);

            _dbContext.AddRange(auctionsToAdd);

            foreach (var auction in auctionsToUpdate)
            {
                var newAuctionState = auctionIdMap[auction.Id];

                if (newAuctionState.ExpectedExpiry < auction.ExpectedExpiry)
                {
                    auction.ExpectedExpiry = newAuctionState.ExpectedExpiry;
                }

                if (newAuctionState.LastSeen > auction.LastSeen)
                {
                    auction.LastSeen = newAuctionState.LastSeen;
                }
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        private async Task LinkItemToAuction(IReadOnlyCollection<Auction> auctionData, CancellationToken cancellationToken)
        {
            var itemIds = new HashSet<long>(auctionData.Select(auc => auc.Item!.Id));

            var items = await _dbContext.Items
                .Where(item => itemIds.Contains(item.Id))
                .ToListAsync(cancellationToken);

            var itemMap = items.ToDictionary(item => item.Id, item => item);

            foreach (var auction in auctionData)
            {
                auction.Item = itemMap.GetValueOrDefault(auction.Item!.Id); // Item can be null as item retrieved from Blizzard API can return 404 #19
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        private async Task AddUnknownItemsAsync(IReadOnlyCollection<Auction> auctionData, CancellationToken cancellationToken)
        {
            var itemIds = auctionData.Select(auc => auc.Item!.Id).Distinct().ToList();

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