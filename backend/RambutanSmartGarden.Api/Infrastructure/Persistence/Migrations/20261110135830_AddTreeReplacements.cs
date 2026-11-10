using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RambutanSmartGarden.Api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTreeReplacements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint(
                name: "AK_Trees_GardenId_Id",
                table: "Trees",
                columns: new[] { "GardenId", "Id" });

            migrationBuilder.CreateTable(
                name: "TreeReplacements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GardenId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OldTreeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NewTreeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlantingSpotId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OldTreePreviousStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ReplacedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PerformedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TreeReplacements", x => x.Id);
                    table.CheckConstraint("CK_TreeReplacements_DifferentTrees", "[OldTreeId] <> [NewTreeId]");
                    table.CheckConstraint("CK_TreeReplacements_OldStatus", "[OldTreePreviousStatus] IN (N'Active', N'Dead')");
                    table.ForeignKey(
                        name: "FK_TreeReplacements_Gardens_GardenId",
                        column: x => x.GardenId,
                        principalTable: "Gardens",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TreeReplacements_PlantingSpots_GardenId_PlantingSpotId",
                        columns: x => new { x.GardenId, x.PlantingSpotId },
                        principalTable: "PlantingSpots",
                        principalColumns: new[] { "GardenId", "Id" });
                    table.ForeignKey(
                        name: "FK_TreeReplacements_Trees_GardenId_NewTreeId",
                        columns: x => new { x.GardenId, x.NewTreeId },
                        principalTable: "Trees",
                        principalColumns: new[] { "GardenId", "Id" });
                    table.ForeignKey(
                        name: "FK_TreeReplacements_Trees_GardenId_OldTreeId",
                        columns: x => new { x.GardenId, x.OldTreeId },
                        principalTable: "Trees",
                        principalColumns: new[] { "GardenId", "Id" });
                    table.ForeignKey(
                        name: "FK_TreeReplacements_Users_PerformedByUserId",
                        column: x => x.PerformedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TreeReplacements_GardenId_NewTreeId",
                table: "TreeReplacements",
                columns: new[] { "GardenId", "NewTreeId" });

            migrationBuilder.CreateIndex(
                name: "IX_TreeReplacements_GardenId_OldTreeId",
                table: "TreeReplacements",
                columns: new[] { "GardenId", "OldTreeId" });

            migrationBuilder.CreateIndex(
                name: "IX_TreeReplacements_GardenId_PlantingSpotId",
                table: "TreeReplacements",
                columns: new[] { "GardenId", "PlantingSpotId" });

            migrationBuilder.CreateIndex(
                name: "IX_TreeReplacements_GardenId_ReplacedAt",
                table: "TreeReplacements",
                columns: new[] { "GardenId", "ReplacedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_TreeReplacements_PerformedByUserId",
                table: "TreeReplacements",
                column: "PerformedByUserId");

            migrationBuilder.CreateIndex(
                name: "UX_TreeReplacements_NewTreeId",
                table: "TreeReplacements",
                column: "NewTreeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_TreeReplacements_OldTreeId",
                table: "TreeReplacements",
                column: "OldTreeId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TreeReplacements");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Trees_GardenId_Id",
                table: "Trees");
        }
    }
}
