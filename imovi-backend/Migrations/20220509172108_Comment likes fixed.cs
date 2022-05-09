using Microsoft.EntityFrameworkCore.Migrations;

namespace imovi_backend.Migrations
{
    public partial class Commentlikesfixed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_LikedComments_CommentId",
                table: "LikedComments",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_LikedComments_UserId",
                table: "LikedComments",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_LikedComments_Comments_CommentId",
                table: "LikedComments",
                column: "CommentId",
                principalTable: "Comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LikedComments_Users_UserId",
                table: "LikedComments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LikedComments_Comments_CommentId",
                table: "LikedComments");

            migrationBuilder.DropForeignKey(
                name: "FK_LikedComments_Users_UserId",
                table: "LikedComments");

            migrationBuilder.DropIndex(
                name: "IX_LikedComments_CommentId",
                table: "LikedComments");

            migrationBuilder.DropIndex(
                name: "IX_LikedComments_UserId",
                table: "LikedComments");
        }
    }
}
