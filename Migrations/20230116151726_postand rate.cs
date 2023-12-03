using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication1.Migrations
{
    public partial class postandrate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServicesPosts_Rates_RateId",
                table: "ServicesPosts");

            migrationBuilder.DropIndex(
                name: "IX_ServicesPosts_RateId",
                table: "ServicesPosts");

            migrationBuilder.DropColumn(
                name: "RateId",
                table: "ServicesPosts");

            migrationBuilder.AddColumn<int>(
                name: "ServicesPostId",
                table: "Rates",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Rates_ServicesPostId",
                table: "Rates",
                column: "ServicesPostId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rates_ServicesPosts_ServicesPostId",
                table: "Rates",
                column: "ServicesPostId",
                principalTable: "ServicesPosts",
                principalColumn: "ServicesPostId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rates_ServicesPosts_ServicesPostId",
                table: "Rates");

            migrationBuilder.DropIndex(
                name: "IX_Rates_ServicesPostId",
                table: "Rates");

            migrationBuilder.DropColumn(
                name: "ServicesPostId",
                table: "Rates");

            migrationBuilder.AddColumn<int>(
                name: "RateId",
                table: "ServicesPosts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ServicesPosts_RateId",
                table: "ServicesPosts",
                column: "RateId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServicesPosts_Rates_RateId",
                table: "ServicesPosts",
                column: "RateId",
                principalTable: "Rates",
                principalColumn: "RateId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
