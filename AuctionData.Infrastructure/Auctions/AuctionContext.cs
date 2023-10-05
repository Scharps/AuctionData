using AuctionData.Domain.Auction;
using Microsoft.EntityFrameworkCore;

namespace AuctionData.Infrastructure.Auctions;

public sealed class AuctionContext : DbContext
{
    public DbSet<Auction> Auctions { get; set; } = null!;

    public AuctionContext(DbContextOptions<AuctionContext> options) : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    }
}