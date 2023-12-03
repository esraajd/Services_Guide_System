using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication1.Migrations
{
    public partial class categories : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ServicesPosts_CategoryId",
                table: "ServicesPosts",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServicesPosts_Categories_CategoryId",
                table: "ServicesPosts",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServicesPosts_Categories_CategoryId",
                table: "ServicesPosts");

            migrationBuilder.DropIndex(
                name: "IX_ServicesPosts_CategoryId",
                table: "ServicesPosts");
        }
    }
}
