using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibrAI.Migrations
{
    /// <inheritdoc />
    public partial class LoansInit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "loans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    user_id = table.Column<string>(type: "TEXT", nullable: false),
                    book_id = table.Column<int>(type: "INTEGER", nullable: false),
                    borrowed_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    due_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    returned_at = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_loans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_loans_books_book_id",
                        column: x => x.book_id,
                        principalTable: "books",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_loans_book",
                table: "loans",
                column: "book_id");

            migrationBuilder.CreateIndex(
                name: "IX_loans_user",
                table: "loans",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "loans");
        }
    }
}
