using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ChessApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialUserScopedSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EmailVerified = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    PasswordHash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    GoogleSubject = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    FailedLoginCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    LockoutUntilUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Provider = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    TokenHash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ExpiresAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Revoked = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OpeningNodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OpeningId = table.Column<int>(type: "integer", nullable: false),
                    ParentNodeId = table.Column<int>(type: "integer", nullable: true),
                    Fen = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    MoveSan = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    LineType = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpeningNodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OpeningNodes_OpeningNodes_ParentNodeId",
                        column: x => x.ParentNodeId,
                        principalTable: "OpeningNodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Openings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Color = table.Column<int>(type: "integer", nullable: false),
                    RootNodeId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Openings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Openings_OpeningNodes_RootNodeId",
                        column: x => x.RootNodeId,
                        principalTable: "OpeningNodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TrainingNodeStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    OpeningNodeId = table.Column<int>(type: "integer", nullable: false),
                    TrainedCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    FailedCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    LastTrainedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    NextDueAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingNodeStats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrainingNodeStats_OpeningNodes_OpeningNodeId",
                        column: x => x.OpeningNodeId,
                        principalTable: "OpeningNodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RepertoireItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Color = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    OpeningId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RepertoireItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RepertoireItems_Openings_OpeningId",
                        column: x => x.OpeningId,
                        principalTable: "Openings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OpeningNodes_OpeningId_Fen",
                table: "OpeningNodes",
                columns: new[] { "OpeningId", "Fen" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OpeningNodes_OpeningId_ParentNodeId",
                table: "OpeningNodes",
                columns: new[] { "OpeningId", "ParentNodeId" });

            migrationBuilder.CreateIndex(
                name: "IX_OpeningNodes_ParentNodeId",
                table: "OpeningNodes",
                column: "ParentNodeId");

            migrationBuilder.CreateIndex(
                name: "IX_Openings_RootNodeId",
                table: "Openings",
                column: "RootNodeId");

            migrationBuilder.CreateIndex(
                name: "IX_Openings_UserId_Color_Name",
                table: "Openings",
                columns: new[] { "UserId", "Color", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_TokenHash",
                table: "RefreshTokens",
                column: "TokenHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RepertoireItems_OpeningId",
                table: "RepertoireItems",
                column: "OpeningId");

            migrationBuilder.CreateIndex(
                name: "IX_RepertoireItems_UserId_Color_Name",
                table: "RepertoireItems",
                columns: new[] { "UserId", "Color", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RepertoireItems_UserId_Color_SortOrder",
                table: "RepertoireItems",
                columns: new[] { "UserId", "Color", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_TrainingNodeStats_OpeningNodeId",
                table: "TrainingNodeStats",
                column: "OpeningNodeId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingNodeStats_UserId",
                table: "TrainingNodeStats",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingNodeStats_UserId_OpeningNodeId",
                table: "TrainingNodeStats",
                columns: new[] { "UserId", "OpeningNodeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OpeningNodes_Openings_OpeningId",
                table: "OpeningNodes",
                column: "OpeningId",
                principalTable: "Openings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OpeningNodes_Openings_OpeningId",
                table: "OpeningNodes");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "RepertoireItems");

            migrationBuilder.DropTable(
                name: "TrainingNodeStats");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Openings");

            migrationBuilder.DropTable(
                name: "OpeningNodes");
        }
    }
}
