using LibraryProjectModule12.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LibraryProjectModule12.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventUser> EventUsers { get; set; }
        public DbSet<Library> Libraries { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Basic relationships and constraints
            builder.Entity<Book>()
                .HasOne(b => b.Author)
                .WithMany(a => a.Books)
                .HasForeignKey(b => b.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Book>()
                .HasOne(b => b.Genre)
                .WithMany(g => g.Books)
                .HasForeignKey(b => b.GenreId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<EventUser>()
                .HasOne(eu => eu.User)
                .WithMany(u => u.EventUsers)
                .HasForeignKey(eu => eu.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<EventUser>()
                .HasOne(eu => eu.Event)
                .WithMany(e => e.EventUsers)
                .HasForeignKey(eu => eu.EventId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Library>()
                .HasOne(l => l.User)
                .WithMany(u => u.Libraries)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Library>()
                .HasOne(l => l.Book)
                .WithMany(b => b.Libraries)
                .HasForeignKey(l => l.BookId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Review>()
                .HasOne(r => r.Library)
                .WithMany(l => l.Reviews)
                .HasForeignKey(r => r.LibraryId)
                .OnDelete(DeleteBehavior.Restrict);

            //Configure Author entity
            builder.Entity<Author>(entity =>

            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.Name).IsRequired().HasMaxLength(128);
                entity.Property(a => a.LastName).IsRequired().HasMaxLength(128);
                entity.Property(a => a.Country).HasMaxLength(128);
                entity.HasQueryFilter(a => !a.IsDeleted);
            });
            // Configure Genre entity
            builder.Entity<Genre>(entity =>
            {
                entity.HasKey(g => g.Id);
                entity.Property(g => g.Name).IsRequired().HasMaxLength(128);
                entity.Property(g => g.Description).IsRequired().HasMaxLength(2048);
                entity.HasQueryFilter(g => !g.IsDeleted);
            });
            // Configure Book entity
            builder.Entity<Book>(entity =>
            {
                entity.HasKey(b => b.Id);
                entity.Property(b => b.Name).IsRequired().HasMaxLength(256);
                entity.Property(b => b.Year).IsRequired();
                entity.Property(b => b.Description).IsRequired().HasMaxLength(2048);
                entity.HasQueryFilter(b => !b.IsDeleted);
            });
            // Configure Event entity
            builder.Entity<Event>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(256);
                entity.Property(e => e.Description).IsRequired().HasMaxLength(2048);
                entity.Property(e => e.Date).IsRequired();
                entity.HasQueryFilter(e => !e.IsDeleted);
            });
            // Configure EventUser entity
            builder.Entity<EventUser>(entity =>
            {
                entity.HasKey(eu => eu.Id);
                entity.Property(eu => eu.UserId).IsRequired();
                entity.Property(eu => eu.EventId).IsRequired();
                entity.HasQueryFilter(eu => !eu.IsDeleted);
            });
            // Configure Library entity
            builder.Entity<Library>(entity =>
            {
                entity.HasKey(l => l.Id);
                entity.Property(l => l.UserId).IsRequired();
                entity.Property(l => l.BookId).IsRequired();
                entity.Property(l => l.ShelfType).IsRequired();
                entity.HasQueryFilter(l => !l.IsDeleted);
            });
            // Configure Review entity
            builder.Entity<Review>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.Property(r => r.LibraryId).IsRequired();
                entity.Property(r => r.Rating).IsRequired();
                entity.Property(r => r.ReviewText).HasMaxLength(2048);
                entity.HasQueryFilter(r => !r.IsDeleted);
            });
            // Configure User entity
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(u => u.Name).IsRequired().HasMaxLength(128);
                entity.Property(u => u.LastName).IsRequired().HasMaxLength(128);
                entity.Property(u => u.DateRegistrated).IsRequired();
                entity.HasQueryFilter(u => !u.IsDeleted);
            });
        }
    }
}
