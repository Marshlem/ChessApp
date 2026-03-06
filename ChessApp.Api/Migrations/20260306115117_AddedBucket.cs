using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChessApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddedBucket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Bucket",
                table: "TrainingNodeStats",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bucket",
                table: "TrainingNodeStats");
        }
    }
}
