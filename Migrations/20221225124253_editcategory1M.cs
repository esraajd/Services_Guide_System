using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication1.Migrations
{
    public partial class editcategory1M : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_ServicesPosts_ServicesPostId",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_ServicesPostId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "SearchTage",
                table: "ServicesPosts");

            migrationBuilder.DropColumn(
                name: "ServicesPostId",
                table: "Categories");

            migrationBuilder.AddColumn<string>(
                name: "SearchTag",
                table: "ServicesPosts",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SearchTag",
                table: "ServicesPosts");

            migrationBuilder.AddColumn<string>(
                name: "SearchTage",
                table: "ServicesPosts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ServicesPostId",
                table: "Categories",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_ServicesPostId",
                table: "Categories",
                column: "ServicesPostId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_ServicesPosts_ServicesPostId",
                table: "Categories",
                column: "ServicesPostId",
                principalTable: "ServicesPosts",
                principalColumn: "ServicesPostId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
