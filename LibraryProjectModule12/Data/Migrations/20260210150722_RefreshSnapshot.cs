using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LibraryProjectModule12.Data.Migrations
{
    /// <inheritdoc />
    public partial class RefreshSnapshot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "EventId",
                table: "EventUsers",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Events",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.InsertData(
                table: "Authors",
                columns: new[] { "Id", "Country", "IsDeleted", "LastName", "Name" },
                values: new object[,]
                {
                    { 1, "British India", false, "Orwell", "George" },
                    { 2, "United Kingdom", false, "Rowling", "J.K." },
                    { 3, "United States", false, "King", "Stephen" },
                    { 4, "Russian SFSR", false, "Asimov", "Isaac" },
                    { 5, "United Kingdom", false, "Christie", "Agatha" },
                    { 6, "Kingdom of Great Britain", false, "Austen", "Jane" },
                    { 7, "United States", false, "Twain", "Mark" },
                    { 8, "United Kingdom", false, "Dickens", "Charles" },
                    { 9, "United States", false, "Hemingway", "Ernest" },
                    { 10, "Kingdom of England", false, "Shakespeare", "William" },
                    { 11, "United Kingdom", false, "Wells", "H.G." },
                    { 12, "Orange Free State", false, "Tolkien", "J.R.R." },
                    { 13, "United States", false, "Lee", "Harper" },
                    { 14, "United States", false, "Fitzgerald", "F. Scott" },
                    { 15, "United States", false, "Bradbury", "Ray" },
                    { 16, "United Kingdom", false, "Pratchett", "Terry" },
                    { 17, "Ireland", false, "Lewis", "C.S." },
                    { 18, "United Kingdom", false, "Doyle", "Arthur Conan" },
                    { 19, "United States", false, "Angelou", "Maya" },
                    { 20, "Russian Empire", false, "Tolstoy", "Leo" },
                    { 21, "United Kingdom", false, "Woolf", "Virginia" },
                    { 22, "Ireland", false, "Wilde", "Oscar" },
                    { 23, "Republic of Florence", false, "Alighieri", "Dante" },
                    { 24, "United States", false, "Dickinson", "Emily" },
                    { 25, "United States", false, "Andrews", "Virginia" },
                    { 26, "Colombia", false, "García Márquez", "Gabriel" },
                    { 27, "Canada", false, "Atwood", "Margaret" },
                    { 28, "Japan", false, "Murakami", "Haruki" },
                    { 29, "French Algeria", false, "Camus", "Albert" },
                    { 30, "United Kingdom", false, "Gaiman", "Neil" },
                    { 31, "United States", false, "Poe", "Edgar Allan" },
                    { 32, "United States", false, "Melville", "Herman" },
                    { 33, "United States", false, "Steinbeck", "John" },
                    { 34, "United States", false, "Plath", "Sylvia" },
                    { 35, "United States", false, "Martin", "George R.R." },
                    { 36, "United States", false, "Vonnegut", "Kurt" },
                    { 37, "United States", false, "Morrison", "Toni" },
                    { 38, "Ireland", false, "Joyce", "James" },
                    { 39, "United States", false, "Salinger", "J.D." },
                    { 40, "United States", false, "Alcott", "Louisa May" },
                    { 41, "United States", false, "Ellison", "Ralph" },
                    { 42, "United States", false, "Hurston", "Zora Neale" },
                    { 43, "Russian Empire", false, "Nabokov", "Vladimir" },
                    { 44, "Austria-Hungary", false, "Kafka", "Franz" },
                    { 45, "United Kingdom", false, "Brontë", "Anne" },
                    { 46, "United Kingdom", false, "Shelley", "Mary" },
                    { 47, "United States", false, "Wharton", "Edith" },
                    { 48, "Poland", false, "Conrad", "Joseph" },
                    { 49, "United States", false, "Faulkner", "William" },
                    { 50, "United States", false, "London", "Jack" },
                    { 51, "France", false, "Dumas", "Alexandre" },
                    { 52, "France", false, "Hugo", "Victor" },
                    { 53, "France", false, "Proust", "Marcel" },
                    { 54, "France", false, "Verne", "Jules" },
                    { 55, "United States", false, "James", "Henry" },
                    { 56, "Scotland", false, "Stevenson", "Robert Louis" },
                    { 57, "United Kingdom", false, "Brontë", "Charlotte" },
                    { 58, "United Kingdom", false, "Brontë", "Emily" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 48);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 49);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 50);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 51);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 52);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 53);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 54);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 55);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 56);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 57);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 58);

            migrationBuilder.AlterColumn<Guid>(
                name: "EventId",
                table: "EventUsers",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Events",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");
        }
    }
}
