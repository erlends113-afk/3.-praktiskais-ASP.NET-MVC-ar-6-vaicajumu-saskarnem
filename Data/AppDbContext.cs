using Microsoft.EntityFrameworkCore;
using EfWebDatabaseApp.Models;

namespace EfWebDatabaseApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Author> Authors => Set<Author>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Book> Books => Set<Book>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Fluent API relācijas un kaskādes dzēšana
            modelBuilder.Entity<Author>()
                .HasMany(a => a.Books)
                .WithOne(b => b.Author)
                .HasForeignKey(b => b.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Category>()
                .HasMany(c => c.Books)
                .WithOne(b => b.Category)
                .HasForeignKey(b => b.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            // Sākotnējie testa dati
            modelBuilder.Entity<Author>().HasData(
                new Author { Id = 1, Name = "Rainis", Email = "rainis@latvija.lv" },
                new Author { Id = 2, Name = "Kārlis Skalbe", Email = "skalbe@latvija.lv" },
                new Author { Id = 3, Name = "Aleksandrs Čaks", Email = "caks@latvija.lv" },
                new Author { Id = 4, Name = "Anšlavs Eglītis", Email = "eglitis@latvija.lv" }
            );

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Title = "Dzeja" },
                new Category { Id = 2, Title = "Pasakas" },
                new Category { Id = 3, Title = "Romāns" }
            );

            modelBuilder.Entity<Book>().HasData(
                new Book { Id = 1, Title = "Zelta zirgs", Price = 12.50m, AuthorId = 1, CategoryId = 1 },
                new Book { Id = 2, Title = "Uguns un nakts", Price = 18.00m, AuthorId = 1, CategoryId = 1 },
                new Book { Id = 3, Title = "Kaķīša dzirnavas", Price = 8.99m, AuthorId = 2, CategoryId = 2 },
                new Book { Id = 4, Title = "Mūžības skartie", Price = 15.20m, AuthorId = 3, CategoryId = 1 },
                new Book { Id = 5, Title = "Homo Novus", Price = 14.50m, AuthorId = 4, CategoryId = 3 },
                new Book { Id = 6, Title = "Pansija pilī", Price = 16.80m, AuthorId = 4, CategoryId = 3 }
            );
        }
    }
}