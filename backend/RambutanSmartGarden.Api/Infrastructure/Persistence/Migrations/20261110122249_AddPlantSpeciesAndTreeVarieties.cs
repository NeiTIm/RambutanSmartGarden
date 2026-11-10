using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RambutanSmartGarden.Api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPlantSpeciesAndTreeVarieties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlantSpecies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ScientificName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlantSpecies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TreeVarieties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlantSpeciesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TreeVarieties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TreeVarieties_PlantSpecies_PlantSpeciesId",
                        column: x => x.PlantSpeciesId,
                        principalTable: "PlantSpecies",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "UX_PlantSpecies_Name",
                table: "PlantSpecies",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_TreeVarieties_PlantSpeciesId_Name",
                table: "TreeVarieties",
                columns: new[] { "PlantSpeciesId", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TreeVarieties");

            migrationBuilder.DropTable(
                name: "PlantSpecies");
        }
    }
}
