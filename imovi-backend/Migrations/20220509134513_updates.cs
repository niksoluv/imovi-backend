using Microsoft.EntityFrameworkCore.Migrations;

namespace imovi_backend.Migrations
{
    public partial class updates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MediaType",
                table: "FavoriteMovies");

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteMovies_MovieId",
                table: "FavoriteMovies",
                column: "MovieId");

            migrationBuilder.AddForeignKey(
                name: "FK_FavoriteMovies_Movies_MovieId",
                table: "FavoriteMovies",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FavoriteMovies_Movies_MovieId",
                table: "FavoriteMovies");

            migrationBuilder.DropIndex(
                name: "IX_FavoriteMovies_MovieId",
                table: "FavoriteMovies");

            migrationBuilder.AddColumn<string>(
                name: "MediaType",
                table: "FavoriteMovies",
                type: "text",
                nullable: true);
        }
    }
}
