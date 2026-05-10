using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChessApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class MoveEvaluation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Evaluation",
                table: "OpeningNodes",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Evaluation",
                table: "OpeningNodes");
        }
    }
}
