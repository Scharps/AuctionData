﻿// <auto-generated />
using System;
using AuctionData.Application.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace AuctionData.ApplicationCore.Migrations
{
    [DbContext(typeof(AuctionDbContext))]
    [Migration("20231020223439_NullableItem")]
    partial class NullableItem
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.11");

            modelBuilder.Entity("AuctionData.Application.Entities.Auction.Auction", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<long?>("Bid")
                        .HasColumnType("INTEGER");

                    b.Property<long>("Buyout")
                        .HasColumnType("INTEGER");

                    b.Property<long>("ConnectedRealmId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTimeOffset>("ExpectedExpiry")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("FirstSeen")
                        .HasColumnType("TEXT");

                    b.Property<string>("InternalBonuses")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<long?>("ItemId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("LastSeen")
                        .HasColumnType("TEXT");

                    b.Property<long>("Quantity")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ConnectedRealmId");

                    b.HasIndex("ItemId");

                    b.ToTable("Auctions");
                });

            modelBuilder.Entity("AuctionData.Application.Entities.Auction.Modifier", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<long?>("AuctionId")
                        .HasColumnType("INTEGER");

                    b.Property<long>("Type")
                        .HasColumnType("INTEGER");

                    b.Property<long>("Value")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("AuctionId");

                    b.ToTable("Modifiers");
                });

            modelBuilder.Entity("AuctionData.Application.Entities.Auction.RegionAndRealmGroup", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("GroupId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Region")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("ConnectedRealms");
                });

            modelBuilder.Entity("AuctionData.Application.Entities.Item.Item", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Items");
                });

            modelBuilder.Entity("AuctionData.Application.Entities.Auction.Auction", b =>
                {
                    b.HasOne("AuctionData.Application.Entities.Auction.RegionAndRealmGroup", "ConnectedRealm")
                        .WithMany()
                        .HasForeignKey("ConnectedRealmId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AuctionData.Application.Entities.Item.Item", "Item")
                        .WithMany()
                        .HasForeignKey("ItemId");

                    b.Navigation("ConnectedRealm");

                    b.Navigation("Item");
                });

            modelBuilder.Entity("AuctionData.Application.Entities.Auction.Modifier", b =>
                {
                    b.HasOne("AuctionData.Application.Entities.Auction.Auction", null)
                        .WithMany("Modifiers")
                        .HasForeignKey("AuctionId");
                });

            modelBuilder.Entity("AuctionData.Application.Entities.Auction.Auction", b =>
                {
                    b.Navigation("Modifiers");
                });
#pragma warning restore 612, 618
        }
    }
}
