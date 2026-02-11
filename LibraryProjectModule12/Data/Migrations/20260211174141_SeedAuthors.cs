using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryProjectModule12.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedAuthors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
            table: "Authors",
            columns: new[] { "Id", "Name", "LastName", "Country", "IsDeleted" },
            values: new object[,]
            {
                { 1, "George", "Orwell", "United Kingdom", false },
                { 2, "Jane", "Austen", "United Kingdom", false },
                { 3, "Fyodor", "Dostoevsky", "Russia", false },
                { 4, "Ernest", "Hemingway", "USA", false },
                { 5, "Ivan", "Vazov", "Bulgaria", false }
            });
        }
        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
            name: "Authors");
        }
    }
}
