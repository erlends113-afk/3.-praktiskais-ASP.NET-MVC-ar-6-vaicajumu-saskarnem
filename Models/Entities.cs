using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EfWebDatabaseApp.Models
{
    [Table("Authors")]
    public class Author
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        public List<Book> Books { get; set; } = new();
    }

    [Table("Categories")]
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Title { get; set; } = string.Empty;

        public List<Book> Books { get; set; } = new();
    }

    [Table("Books")]
    public class Book
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Title { get; set; } = string.Empty;

        [Range(0.01, 1000.00)]
        public decimal Price { get; set; }

        public int AuthorId { get; set; }
        public Author? Author { get; set; }

        public int CategoryId { get; set; }
        public Category? Category { get; set; }
    }
}