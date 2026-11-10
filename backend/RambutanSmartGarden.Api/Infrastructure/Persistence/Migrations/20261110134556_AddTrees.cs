using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RambutanSmartGarden.Api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTrees : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint(
                name: "AK_TreeVarieties_PlantSpeciesId_Id",
                table: "TreeVarieties",
                columns: new[] { "PlantSpeciesId", "Id" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_PlantingSpots_GardenId_Id",
                table: "PlantingSpots",
                columns: new[] { "GardenId", "Id" });

            migrationBuilder.CreateTable(
                name: "Trees",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GardenId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlantSpeciesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TreeVarietyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PlantingSpotId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    PlantedAt = table.Column<DateOnly>(type: "date", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trees", x => x.Id);
                    table.CheckConstraint("CK_Trees_Status", "[Status] IN (N'Active', N'Dead', N'Removed')");
                    table.ForeignKey(
                        name: "FK_Trees_Gardens_GardenId",
                        column: x => x.GardenId,
                        principalTable: "Gardens",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Trees_PlantSpecies_PlantSpeciesId",
                        column: x => x.PlantSpeciesId,
                        principalTable: "PlantSpecies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Trees_PlantingSpots_GardenId_PlantingSpotId",
                        columns: x => new { x.GardenId, x.PlantingSpotId },
                        principalTable: "PlantingSpots",
                        principalColumns: new[] { "GardenId", "Id" });
                    table.ForeignKey(
                        name: "FK_Trees_TreeVarieties_PlantSpeciesId_TreeVarietyId",
                        columns: x => new { x.PlantSpeciesId, x.TreeVarietyId },
                        principalTable: "TreeVarieties",
                        principalColumns: new[] { "PlantSpeciesId", "Id" });
                });

            migrationBuilder.CreateIndex(
                name: "IX_Trees_GardenId_PlantingSpotId",
                table: "Trees",
                columns: new[] { "GardenId", "PlantingSpotId" });

            migrationBuilder.CreateIndex(
                name: "IX_Trees_PlantSpeciesId_TreeVarietyId",
                table: "Trees",
                columns: new[] { "PlantSpeciesId", "TreeVarietyId" });

            migrationBuilder.CreateIndex(
                name: "UX_Trees_Active_PlantingSpotId",
                table: "Trees",
                column: "PlantingSpotId",
                unique: true,
                filter: "[PlantingSpotId] IS NOT NULL AND [Status] = N'Active'");

            migrationBuilder.CreateIndex(
                name: "UX_Trees_GardenId_Code",
                table: "Trees",
                columns: new[] { "GardenId", "Code" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Trees");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_TreeVarieties_PlantSpeciesId_Id",
                table: "TreeVarieties");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_PlantingSpots_GardenId_Id",
                table: "PlantingSpots");
        }
    }
}
