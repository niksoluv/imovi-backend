using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace imovi_backend.Migrations
{
    public partial class CustomListsMigration2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomList_Users_UserId",
                table: "CustomList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CustomList",
                table: "CustomList");

            migrationBuilder.RenameTable(
                name: "CustomList",
                newName: "CustomLists");

            migrationBuilder.RenameIndex(
                name: "IX_CustomList_UserId",
                table: "CustomLists",
                newName: "IX_CustomLists_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CustomLists",
                table: "CustomLists",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "CustomListsMovies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MovieId = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomListId = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomListsMovies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomListsMovies_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomListsMovies_MovieId",
                table: "CustomListsMovies",
                column: "MovieId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomLists_Users_UserId",
                table: "CustomLists",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomLists_Users_UserId",
                table: "CustomLists");

            migrationBuilder.DropTable(
                name: "CustomListsMovies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CustomLists",
                table: "CustomLists");

            migrationBuilder.RenameTable(
                name: "CustomLists",
                newName: "CustomList");

            migrationBuilder.RenameIndex(
                name: "IX_CustomLists_UserId",
                table: "CustomList",
                newName: "IX_CustomList_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CustomList",
                table: "CustomList",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomList_Users_UserId",
                table: "CustomList",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
