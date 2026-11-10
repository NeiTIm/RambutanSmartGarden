using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RambutanSmartGarden.Api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPlantingSpots : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlantingSpots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GardenId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GardenAreaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Latitude = table.Column<decimal>(type: "decimal(9,6)", nullable: true),
                    Longitude = table.Column<decimal>(type: "decimal(9,6)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlantingSpots", x => x.Id);
                    table.CheckConstraint("CK_PlantingSpots_Coordinates", "(\r\n        ([Latitude] IS NULL AND [Longitude] IS NULL)\r\n        OR\r\n        (\r\n            [Latitude] IS NOT NULL\r\n            AND [Longitude] IS NOT NULL\r\n            AND [Latitude] BETWEEN -90 AND 90\r\n            AND [Longitude] BETWEEN -180 AND 180\r\n        )\r\n    )");
                    table.ForeignKey(
                        name: "FK_PlantingSpots_GardenAreas_GardenAreaId",
                        column: x => x.GardenAreaId,
                        principalTable: "GardenAreas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PlantingSpots_Gardens_GardenId",
                        column: x => x.GardenId,
                        principalTable: "Gardens",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlantingSpots_GardenAreaId",
                table: "PlantingSpots",
                column: "GardenAreaId");

            migrationBuilder.CreateIndex(
                name: "UX_PlantingSpots_GardenId_Code",
                table: "PlantingSpots",
                columns: new[] { "GardenId", "Code" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlantingSpots");
        }
    }
}
