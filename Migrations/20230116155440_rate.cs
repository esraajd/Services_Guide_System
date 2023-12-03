using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication1.Migrations
{
    public partial class rate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StarsPoints",
                table: "Rates");

            migrationBuilder.AddColumn<int>(
                name: "Like",
                table: "Rates",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Like",
                table: "Rates");

            migrationBuilder.AddColumn<int>(
                name: "StarsPoints",
                table: "Rates",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
