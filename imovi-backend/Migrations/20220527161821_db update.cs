using Microsoft.EntityFrameworkCore.Migrations;

namespace imovi_backend.Migrations
{
    public partial class dbupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_CustomListsMovies_CustomListId",
                table: "CustomListsMovies",
                column: "CustomListId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomListsMovies_CustomLists_CustomListId",
                table: "CustomListsMovies",
                column: "CustomListId",
                principalTable: "CustomLists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomListsMovies_CustomLists_CustomListId",
                table: "CustomListsMovies");

            migrationBuilder.DropIndex(
                name: "IX_CustomListsMovies_CustomListId",
                table: "CustomListsMovies");
        }
    }
}
