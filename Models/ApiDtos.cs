namespace EfWebDatabaseApp.Models
{
    // 1. POST vaicājumam
    public class BookSearchDto
    {
        public string Keyword { get; set; } = string.Empty;
    }

    // 2. POST vaicājumam
    public class PriceFilterDto
    {
        public decimal MaxPrice { get; set; }
    }

    // 3. POST vaicājumam
    public class AuthorSearchDto
    {
        public string AuthorName { get; set; } = string.Empty;
    }
}