using Microsoft.EntityFrameworkCore;
using AuctionData.Application.Entities.Auction;

namespace AuctionData.Application.Data;

public sealed class AuctionDbContext : DbContext
{
    public DbSet<AuctionLog> AuctionLogs { get; set; } = null!;

    public AuctionDbContext(DbContextOptions<AuctionDbContext> options) : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    }
}