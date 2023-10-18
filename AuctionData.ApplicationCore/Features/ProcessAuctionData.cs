using AuctionData.Application.Data;
using AuctionData.Application.Entities.Auction;
using AuctionData.Application.Services.BlizzardApi;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

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

            _dbContext.AuctionLogs.AddRange(auctionData);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        private async Task LinkAssociatedItemListings(IReadOnlyCollection<AuctionLog> auctionData, CancellationToken cancellationToken)
        {
            var auctionIds = auctionData.Select(log => log.Auction.Id).ToList();

            var existingAuctions = _dbContext.AuctionLogs
                .Where(log => auctionIds.Contains(log.Auction.Id))
                .Select(log => log.Auction)
                .GroupBy(auc => auc.Id)
                .Select(group => group.First());

            var existingAuctionsDictionary = await existingAuctions.ToDictionaryAsync(auc => auc.Id, auc => auc.ItemListing.Id, cancellationToken);


            foreach (var al in auctionData)
            {
                al.Auction.ItemListing.Id = existingAuctionsDictionary.GetValueOrDefault(al.Auction.Id);
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