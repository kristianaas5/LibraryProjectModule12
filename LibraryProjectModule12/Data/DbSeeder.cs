using LibraryProjectModule12.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryProjectModule12.Data
{
    /// <summary>
    /// DbSeeder is a utility class responsible
    /// </summary>
    public static class DbSeeder
    {
        /// Seeds the database with initial data for Authors.
        public static async Task SeedAuthorsAsync(ApplicationDbContext context)
        {
            if (await context.Authors.AnyAsync())// If there are already authors in the database, skip seeding.
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
        /// Seeds the database with initial data for Genres.
        public static async Task SeedGenresAsync(ApplicationDbContext context)
        {
            if (await context.Genres.AnyAsync())// If there are already genres in the database, skip seeding.
                return;

            var genres = new List<Genre>
            {
                new Genre
                {
                    Name = "Fantasy",
                    Description = "Fiction with magical or supernatural elements.",
                    IsDeleted = false
                },
                new Genre
                {
                    Name = "Science Fiction",
                    Description = "Stories based on futuristic science and advanced technology.",
                    IsDeleted = false
                },
                new Genre
                {
                    Name = "Drama",
                    Description = "Serious narratives focused on emotional themes and conflicts.",
                    IsDeleted = false
                },
                new Genre
                {
                    Name = "Romance",
                    Description = "Stories centered around love and relationships.",
                    IsDeleted = false
                },
                new Genre
                {
                    Name = "Thriller",
                    Description = "Fast-paced stories full of suspense and tension.",
                    IsDeleted = false
                },
                new Genre
                {
                    Name = "Horror",
                    Description = "Stories intended to scare or create fear.",
                    IsDeleted = false
                },
                new Genre
                {
                    Name = "Historical",
                    Description = "Fiction set in a real historical period or inspired by real events.",
                    IsDeleted = false
                }
            };

            await context.Genres.AddRangeAsync(genres);
            await context.SaveChangesAsync();
        }

        /// Seeds the database with initial data for Events.
        public static async Task SeedEventsAsync(ApplicationDbContext context)
        {
            if (await context.Events.AnyAsync())
                return;

            var events = new List<Event>
            {
                new Event
                {
                    Name = "Book Reading Night",
                    Description = "An evening dedicated to reading and discussing classic literature.",
                    Date = new DateTime(2026, 3, 15, 18, 30, 0),
                    IsDeleted = false
                },
                new Event
                {
                    Name = "Fantasy Book Club",
                    Description = "Monthly meeting focused on popular fantasy novels and authors.",
                    Date = new DateTime(2026, 4, 5, 17, 0, 0),
                    IsDeleted = false
                },
                new Event
                {
                    Name = "Author Meet & Greet",
                    Description = "Special guest author session including Q&A and book signing.",
                    Date = new DateTime(2026, 5, 10, 16, 0, 0),
                    IsDeleted = false
                },
                new Event
                {
                    Name = "Creative Writing Workshop",
                    Description = "Interactive workshop for aspiring writers.",
                    Date = new DateTime(2026, 6, 20, 14, 0, 0),
                    IsDeleted = false
                },
                new Event
                {
                    Name = "Summer Literature Festival",
                    Description = "Annual festival celebrating literature, authors, and readers.",
                    Date = new DateTime(2026, 7, 12, 12, 0, 0),
                    IsDeleted = false
                }
            };

            await context.Events.AddRangeAsync(events);
            await context.SaveChangesAsync();
        }

        // Seeds the database with initial data for Books.
        public static async Task SeedBooksAsync(ApplicationDbContext context)
        {
            if (await context.Books.AnyAsync())// If there are already books in the database, skip seeding.
            return;

            var books = new List<Book>
            {
                new Book
                {
                    Name = "1984",
                    Year = 1949,
                    AuthorId = 1, // George Orwell
                    GenreId = 2,  // Science Fiction
                    Description = "A dystopian novel exploring totalitarianism, surveillance, and loss of individuality.",
                    IsDeleted = false
                },
                new Book
                {
                    Name = "Pride and Prejudice",
                    Year = 1813,
                    AuthorId = 2, // Jane Austen
                    GenreId = 4,  // Romance
                    Description = "A classic romantic novel that critiques social class and marriage in 19th century England.",
                    IsDeleted = false
                },
                new Book
                {
                    Name = "Crime and Punishment",
                    Year = 1866,
                    AuthorId = 3, // Fyodor Dostoevsky
                    GenreId = 3,  // Drama
                    Description = "A psychological novel examining guilt, morality, and redemption.",
                    IsDeleted = false
                },
                new Book
                {
                    Name = "The Old Man and the Sea",
                    Year = 1952,
                    AuthorId = 4, // Ernest Hemingway
                    GenreId = 5,  // Drama
                    Description = "A short novel about perseverance and the struggle between man and nature.",
                    IsDeleted = false
                },
                new Book
                {
                    Name = "Harry Potter and the Philosopher's Stone",
                    Year = 1997,
                    AuthorId = 5, // J.K. Rowling
                    GenreId = 1,  // Fantasy
                    Description = "A fantasy novel about a young wizard discovering his magical heritage.",
                    IsDeleted = false
                }
            };

            await context.Books.AddRangeAsync(books);
            await context.SaveChangesAsync();
        }

    }
}
