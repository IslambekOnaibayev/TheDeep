using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheDeep.Infrastructure.Data.Migrations
{

    public partial class InitialCreate : Migration
    {

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GameRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WinnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    LoserId = table.Column<Guid>(type: "uuid", nullable: false),
                    WinnerName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    LoserName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    GridWidth = table.Column<int>(type: "integer", nullable: false),
                    GridHeight = table.Column<int>(type: "integer", nullable: false),
                    EndedByForfeit = table.Column<bool>(type: "boolean", nullable: false),
                    FinishedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    GamesPlayed = table.Column<int>(type: "integer", nullable: false),
                    Wins = table.Column<int>(type: "integer", nullable: false),
                    Losses = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastSeenAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameRecords_FinishedAt",
                table: "GameRecords",
                column: "FinishedAt");

            migrationBuilder.CreateIndex(
                name: "IX_GameRecords_LoserId",
                table: "GameRecords",
                column: "LoserId");

            migrationBuilder.CreateIndex(
                name: "IX_GameRecords_WinnerId",
                table: "GameRecords",
                column: "WinnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Players_Wins",
                table: "Players",
                column: "Wins");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameRecords");

            migrationBuilder.DropTable(
                name: "Players");
        }
    }
}
