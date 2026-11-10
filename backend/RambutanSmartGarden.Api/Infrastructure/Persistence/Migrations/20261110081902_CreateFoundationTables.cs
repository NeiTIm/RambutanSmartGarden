using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RambutanSmartGarden.Api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CreateFoundationTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Gardens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    LocationDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AreaSquareMeters = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gardens", x => x.Id);
                    table.CheckConstraint("CK_Gardens_AreaSquareMeters", "[AreaSquareMeters] IS NULL OR [AreaSquareMeters] > 0");
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NormalizedUsername = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PhoneNumber = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    NormalizedPhoneNumber = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsSystemAdmin = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActorUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GardenId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Action = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    EntityType = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    EntityId = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    OldValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IpAddress = table.Column<string>(type: "varchar(45)", unicode: false, maxLength: 45, nullable: true),
                    CorrelationId = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    OccurredAt = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                    table.CheckConstraint("CK_AuditLogs_NewValues_IsJson", "[NewValues] IS NULL OR ISJSON([NewValues]) = 1");
                    table.CheckConstraint("CK_AuditLogs_OldValues_IsJson", "[OldValues] IS NULL OR ISJSON([OldValues]) = 1");
                    table.ForeignKey(
                        name: "FK_AuditLogs_Gardens_GardenId",
                        column: x => x.GardenId,
                        principalTable: "Gardens",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AuditLogs_Users_ActorUserId",
                        column: x => x.ActorUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GardenMemberships",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GardenId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Role = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GardenMemberships", x => x.Id);
                    table.CheckConstraint("CK_GardenMemberships_Role", "[Role] IN ('Admin', 'Member', 'Viewer')");
                    table.ForeignKey(
                        name: "FK_GardenMemberships_Gardens_GardenId",
                        column: x => x.GardenId,
                        principalTable: "Gardens",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GardenMemberships_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PasswordResetRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PerformedByAdminId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ResetAt = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordResetRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PasswordResetRecords_Users_PerformedByAdminId",
                        column: x => x.PerformedByAdminId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PasswordResetRecords_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RefreshTokenHash = table.Column<string>(type: "varchar(128)", unicode: false, maxLength: 128, nullable: true),
                    DeviceName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IpAddress = table.Column<string>(type: "varchar(45)", unicode: false, maxLength: 45, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false),
                    LastSeenAt = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true),
                    RevokedAt = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true),
                    RevokedReason = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSessions", x => x.Id);
                    table.CheckConstraint("CK_UserSessions_ExpiresAt", "[ExpiresAt] > [CreatedAt]");
                    table.CheckConstraint("CK_UserSessions_RevokedAt", "[RevokedAt] IS NULL OR [RevokedAt] >= [CreatedAt]");
                    table.ForeignKey(
                        name: "FK_UserSessions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_ActorUserId_OccurredAt",
                table: "AuditLogs",
                columns: new[] { "ActorUserId", "OccurredAt" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_EntityType_EntityId_OccurredAt",
                table: "AuditLogs",
                columns: new[] { "EntityType", "EntityId", "OccurredAt" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_GardenId_OccurredAt",
                table: "AuditLogs",
                columns: new[] { "GardenId", "OccurredAt" });

            migrationBuilder.CreateIndex(
                name: "IX_GardenMemberships_UserId",
                table: "GardenMemberships",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "UX_GardenMemberships_GardenId_UserId",
                table: "GardenMemberships",
                columns: new[] { "GardenId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PasswordResetRecords_PerformedByAdminId",
                table: "PasswordResetRecords",
                column: "PerformedByAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_PasswordResetRecords_UserId_ResetAt",
                table: "PasswordResetRecords",
                columns: new[] { "UserId", "ResetAt" });

            migrationBuilder.CreateIndex(
                name: "UX_Users_NormalizedPhoneNumber",
                table: "Users",
                column: "NormalizedPhoneNumber",
                unique: true,
                filter: "[NormalizedPhoneNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UX_Users_NormalizedUsername",
                table: "Users",
                column: "NormalizedUsername",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_ExpiresAt",
                table: "UserSessions",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_UserId",
                table: "UserSessions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "UX_UserSessions_RefreshTokenHash",
                table: "UserSessions",
                column: "RefreshTokenHash",
                unique: true,
                filter: "[RefreshTokenHash] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "GardenMemberships");

            migrationBuilder.DropTable(
                name: "PasswordResetRecords");

            migrationBuilder.DropTable(
                name: "UserSessions");

            migrationBuilder.DropTable(
                name: "Gardens");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
