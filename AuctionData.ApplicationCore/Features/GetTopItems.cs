using AuctionData.Application.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AuctionData.Application.Features;

public static class GetTopItems
{
    public record GetTopItemsCommand : IRequest<TopItemsResult>;

    public record TopItemsResult(ICollection<long> Items);

    public sealed class GetTopItemsHandler : IRequestHandler<GetTopItemsCommand, TopItemsResult>
    {
        private readonly AuctionDbContext _dbContext;

        public GetTopItemsHandler(AuctionDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<TopItemsResult> Handle(GetTopItemsCommand request, CancellationToken cancellationToken)
        {
            var query = _dbContext.Auctions
                        .GroupBy(auc => auc.Item.Id)
                        .Select(itemAuctions => new { Item = itemAuctions.Key, MarketCap = itemAuctions.Sum(auc => auc.Buyout) })
                        .OrderByDescending(itemAuc => itemAuc.MarketCap)
                        .Take(10)
                        .Select(itemAuctions => itemAuctions.Item);

            return new TopItemsResult(await query.ToArrayAsync());
        }
    }
}