using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication1.Migrations
{
    public partial class PostWithCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CategoryPosts",
                columns: table => new
                {
                    CategoryPostsId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PostId = table.Column<int>(nullable: false),
                    ServicesPostId = table.Column<int>(nullable: true),
                    CategoryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryPosts", x => x.CategoryPostsId);
                    table.ForeignKey(
                        name: "FK_CategoryPosts_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoryPosts_ServicesPosts_ServicesPostId",
                        column: x => x.ServicesPostId,
                        principalTable: "ServicesPosts",
                        principalColumn: "ServicesPostId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoryPosts_CategoryId",
                table: "CategoryPosts",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryPosts_ServicesPostId",
                table: "CategoryPosts",
                column: "ServicesPostId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoryPosts");
        }
    }
}
