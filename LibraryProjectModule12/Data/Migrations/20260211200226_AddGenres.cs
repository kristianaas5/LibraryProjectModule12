using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryProjectModule12.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddGenres : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
            table: "Genres",
            columns: new[] { "Id", "Name", "Description", "IsDeleted" },
            values: new object[,]
            {
                { 1, "Fantasy", "Fiction with magical or supernatural elements.", false },
                { 2, "Science Fiction", "Fiction based on futuristic science and technology.", false },
                { 3, "Drama", "Serious narratives focused on emotional themes.", false },
                { 4, "Romance", "Stories centered around love and relationships.", false },
                { 5, "Thriller", "Fast-paced stories full of suspense and tension.", false },
                { 6, "Horror", "Stories intended to scare or create fear.", false },
                { 7, "Historical", "Fiction set in a real historical period.", false }
            });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
            table: "Genres",
            keyColumn: "Id",
            keyValues: new object[] { 1, 2, 3, 4, 5, 6, 7 });

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Genres");
        }
    }
}
