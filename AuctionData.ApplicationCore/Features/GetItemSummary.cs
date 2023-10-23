using AuctionData.Application.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AuctionData.Application.Features;

public static class GetItemSummary
{
    public record GetItemSummaryCommand(long ItemId) : IRequest<ItemSummaryResult>;

    public record ItemSummaryResult(long ItemId,
                                    string Name,
                                    long CurrentPrice,
                                    long MedianPrice,
                                    long TotalQuantity,
                                    long MarketCap);

    public sealed class GetItemSummaryHandler : IRequestHandler<GetItemSummaryCommand, ItemSummaryResult?>
    {
        private readonly AuctionDbContext _dbContext;

        public GetItemSummaryHandler(AuctionDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<ItemSummaryResult?> Handle(GetItemSummaryCommand request, CancellationToken cancellationToken)
        {
            const string PlaceHolderName = "Place holder";

            var itemSummaryQuery = _dbContext.Auctions
                                        .Where(auc => auc.Item != null && auc.Item.Id == request.ItemId)
                                        .GroupBy(auc => auc.Item!.Id)
                                        .Select(itemAuctions => new ItemSummaryResult(
                                            itemAuctions.Key,
                                            PlaceHolderName,
                                            itemAuctions.Min(auc => auc.Buyout / auc.Quantity),
                                            -1,
                                            itemAuctions.Sum(auc => auc.Quantity),
                                            itemAuctions.Sum(auc => auc.Buyout)));
            return itemSummaryQuery.SingleOrDefaultAsync(cancellationToken);
        }
    }
}