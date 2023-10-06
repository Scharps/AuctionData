using AuctionData.Domain.Auction;
using Microsoft.EntityFrameworkCore;

namespace AuctionData.Application.Data;

public sealed class AuctionDbContext : DbContext
{
    public DbSet<Auction> Auctions { get; set; } = null!;

    public AuctionDbContext(DbContextOptions<AuctionDbContext> options) : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    }
}