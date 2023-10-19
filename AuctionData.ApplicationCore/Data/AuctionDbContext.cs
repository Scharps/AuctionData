using Microsoft.EntityFrameworkCore;
using AuctionData.Application.Entities.Auction;
using AuctionData.Application.Entities.Item;

namespace AuctionData.Application.Data;

public sealed class AuctionDbContext : DbContext
{
    public DbSet<Auction> Auctions { get; set; } = null!;
    public DbSet<Item> Items { get; set; } = null!;
    public DbSet<Modifier> Modifiers { get; set; } = null!;
    public DbSet<RegionAndRealmGroup> ConnectedRealms { get; set; } = null!;

    public AuctionDbContext(DbContextOptions<AuctionDbContext> options) : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    }
}