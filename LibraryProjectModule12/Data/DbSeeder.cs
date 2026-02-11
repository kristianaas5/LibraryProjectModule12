using LibraryProjectModule12.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryProjectModule12.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAuthorsAsync(ApplicationDbContext context)
        {
            if (await context.Authors.AnyAsync())
                return;

            var authors = new List<Author>
            {
                new Author
                {
                    Name = "George",
                    LastName = "Orwell",
                    Country = "United Kingdom",
                    IsDeleted = false
                },
                new Author
                {
                    Name = "Jane",
                    LastName = "Austen",
                    Country = "United Kingdom",
                    IsDeleted = false
                },
                new Author
                {
                    Name = "Fyodor",
                    LastName = "Dostoevsky",
                    Country = "Russia",
                    IsDeleted = false
                },
                new Author
                {
                    Name = "Ernest",
                    LastName = "Hemingway",
                    Country = "USA",
                    IsDeleted = false
                },
                new Author
                {
                    Name = "J.K.",
                    LastName = "Rowling",
                    Country = "United Kingdom",
                    IsDeleted = false
                }
            };

            await context.Authors.AddRangeAsync(authors);
            await context.SaveChangesAsync();
        }
        public static async Task SeedGenresAsync(ApplicationDbContext context)
        {
            if (await context.Genres.AnyAsync())
                return;

            var genres = new List<Genre>
    {
        new Genre { Name = "Fantasy", IsDeleted = false },
        new Genre { Name = "Science Fiction", IsDeleted = false },
        new Genre { Name = "Drama", IsDeleted = false },
        new Genre { Name = "Romance", IsDeleted = false },
        new Genre { Name = "Thriller", IsDeleted = false },
        new Genre { Name = "Horror", IsDeleted = false },
        new Genre { Name = "Historical", IsDeleted = false }
    };

            await context.Genres.AddRangeAsync(genres);
            await context.SaveChangesAsync();
        }
    }
}
