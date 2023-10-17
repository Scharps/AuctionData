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

            await AddUnknownItems(auctionData, cancellationToken);

            await LinkAssociatedItemListings(auctionData, cancellationToken);

            _dbContext.AddRange(auctionData, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        private async Task LinkAssociatedItemListings(IReadOnlyCollection<AuctionLog> auctionData, CancellationToken cancellationToken)
        {
            var oldAndNew = await _dbContext.AuctionLogs
                .Join(
                    auctionData,
                    logs => logs.Auction.Id,
                    data => data.Auction.Id,
                    (existing, @new) => new
                    {
                        Existing = existing.Auction,
                        New = @new.Auction
                    }
                )
                .ToListAsync();
            foreach (var pair in oldAndNew)
            {
                pair.New.ItemListing = pair.Existing.ItemListing;
            }
        }

        private async Task AddUnknownItems(IReadOnlyCollection<AuctionLog> auctionData, CancellationToken cancellationToken)
        {
            var itemIds = auctionData.Select(auc => auc.Auction.ItemListing.ItemId).Distinct().ToList();

            var knownItemIds = await _dbContext.Items.Where(item => itemIds.Contains(item.Id)).Select(item => item.Id).ToListAsync();

            var unknownIds = itemIds.Except(knownItemIds);

            await foreach (var item in _client.GetItemsAsync(unknownIds, 5))
            {
                _dbContext.Items.Add(item);
            }
        }
    }

    public sealed class ProcessAuctionDataHostedService : IHostedService, IDisposable
    {
        private Timer? _timer = null!;
        private IServiceProvider _serviceProvider;
        private IMediator _mediator;

        public ProcessAuctionDataHostedService(IMediator mediator, IServiceProvider serviceProvider)
        {
            _mediator = mediator;
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
            await using (var scope = _serviceProvider.CreateAsyncScope())
            {
                await _mediator.Send(new ProcessAuctionDataCommand(1305));
            }
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