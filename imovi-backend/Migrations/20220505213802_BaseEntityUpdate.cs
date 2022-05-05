using Microsoft.EntityFrameworkCore.Migrations;

namespace imovi_backend.Migrations
{
    public partial class BaseEntityUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RegisterDate",
                table: "Users",
                newName: "Date");

            migrationBuilder.RenameColumn(
                name: "RegisterDate",
                table: "UserMovieHistories",
                newName: "Date");

            migrationBuilder.RenameColumn(
                name: "RegisterDate",
                table: "Movies",
                newName: "Date");

            migrationBuilder.RenameColumn(
                name: "RegisterDate",
                table: "FavoriteMovies",
                newName: "Date");

            migrationBuilder.RenameColumn(
                name: "RegisterDate",
                table: "Comments",
                newName: "Date");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Users",
                newName: "RegisterDate");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "UserMovieHistories",
                newName: "RegisterDate");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Movies",
                newName: "RegisterDate");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "FavoriteMovies",
                newName: "RegisterDate");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Comments",
                newName: "RegisterDate");
        }
    }
}
