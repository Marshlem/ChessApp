using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChessApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class RemovedColor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Openings_UserId_Color_Name",
                table: "Openings");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "Openings");

            migrationBuilder.CreateIndex(
                name: "IX_Openings_UserId_Name",
                table: "Openings",
                columns: new[] { "UserId", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Openings_UserId_Name",
                table: "Openings");

            migrationBuilder.AddColumn<int>(
                name: "Color",
                table: "Openings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Openings_UserId_Color_Name",
                table: "Openings",
                columns: new[] { "UserId", "Color", "Name" },
                unique: true);
        }
    }
}
