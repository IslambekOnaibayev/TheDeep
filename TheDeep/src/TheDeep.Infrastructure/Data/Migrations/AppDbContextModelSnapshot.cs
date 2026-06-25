
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using TheDeep.Infrastructure.Data;

#nullable disable

namespace TheDeep.Infrastructure.Data.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "10.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("TheDeep.Core.GameRecordAggregate.GameRecord", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<bool>("EndedByForfeit")
                        .HasColumnType("boolean");

                    b.Property<DateTimeOffset>("FinishedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("GridHeight")
                        .HasColumnType("integer");

                    b.Property<int>("GridWidth")
                        .HasColumnType("integer");

                    b.Property<Guid>("LoserId")
                        .HasColumnType("uuid");

                    b.Property<string>("LoserName")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.Property<Guid>("WinnerId")
                        .HasColumnType("uuid");

                    b.Property<string>("WinnerName")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.HasKey("Id");

                    b.HasIndex("FinishedAt");

                    b.HasIndex("LoserId");

                    b.HasIndex("WinnerId");

                    b.ToTable("GameRecords");
                });

            modelBuilder.Entity("TheDeep.Core.PlayerAggregate.Player", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("GamesPlayed")
                        .HasColumnType("integer");

                    b.Property<DateTimeOffset>("LastSeenAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Losses")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(24)
                        .HasColumnType("character varying(24)");

                    b.Property<int>("Wins")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("Wins");

                    b.ToTable("Players");
                });
#pragma warning restore 612, 618
        }
    }
}
