using Microsoft.EntityFrameworkCore.Migrations;

namespace imovi_backend.Migrations
{
    public partial class ManyToManyUserHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_UserMovieHistories_MovieId",
                table: "UserMovieHistories",
                column: "MovieId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserMovieHistories_Movies_MovieId",
                table: "UserMovieHistories",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserMovieHistories_Movies_MovieId",
                table: "UserMovieHistories");

            migrationBuilder.DropIndex(
                name: "IX_UserMovieHistories_MovieId",
                table: "UserMovieHistories");
        }
    }
}
