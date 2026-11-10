using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RambutanSmartGarden.Api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddGardenAreas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GardenAreas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GardenId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    AreaSquareMeters = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GardenAreas", x => x.Id);
                    table.CheckConstraint("CK_GardenAreas_AreaSquareMeters", "[AreaSquareMeters] IS NULL OR [AreaSquareMeters] > 0");
                    table.ForeignKey(
                        name: "FK_GardenAreas_Gardens_GardenId",
                        column: x => x.GardenId,
                        principalTable: "Gardens",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "UX_GardenAreas_GardenId_Name",
                table: "GardenAreas",
                columns: new[] { "GardenId", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GardenAreas");
        }
    }
}
