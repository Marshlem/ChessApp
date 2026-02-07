using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChessApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class MoveUci : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MoveUci",
                table: "OpeningNodes",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MoveUci",
                table: "OpeningNodes");
        }
    }
}
