using EfWebDatabaseApp.Data;
using EfWebDatabaseApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EfWebDatabaseApp.Controllers
{
    [ApiController]
    [Route("api")]
    public class ApiController : ControllerBase
    {
        private readonly AppDbContext _db;

        public ApiController(AppDbContext db)
        {
            _db = db;
        }

        // ============================================================
        // 1. DAĻA: 3 POST GALAPUNKTI (ar datu ievadi)
        // ============================================================

        // POST: api/books/search
        // 1. Meklēt grāmatas pēc nosaukuma atslēgvārda
        [HttpPost("books/search")]
        public async Task<IActionResult> SearchBooks([FromBody] BookSearchDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Keyword))
                return BadRequest(new { message = "Lūdzu, ievadiet meklējamo vārdu!" });

            var books = await _db.Books
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Where(b => b.Title.ToLower().Contains(dto.Keyword.ToLower()))
                .Select(b => new
                {
                    b.Id,
                    b.Title,
                    b.Price,
                    Author = b.Author != null ? b.Author.Name : null,
                    Category = b.Category != null ? b.Category.Title : null
                })
                .ToListAsync();

            return Ok(books);
        }

        // POST: api/books/filter-by-price
        // 2. Atlasīt grāmatas ar cenu līdz noteiktai summai
        [HttpPost("books/filter-by-price")]
        public async Task<IActionResult> FilterByPrice([FromBody] PriceFilterDto dto)
        {
            var books = await _db.Books
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Where(b => b.Price <= dto.MaxPrice)
                .OrderBy(b => b.Price)
                .Select(b => new
                {
                    b.Id,
                    b.Title,
                    b.Price,
                    Author = b.Author != null ? b.Author.Name : null,
                    Category = b.Category != null ? b.Category.Title : null
                })
                .ToListAsync();

            return Ok(books);
        }

        // POST: api/authors/search
        // 3. Meklēt autoru un iegūt tā grāmatu sarakstu
        [HttpPost("authors/search")]
        public async Task<IActionResult> SearchAuthor([FromBody] AuthorSearchDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.AuthorName))
                return BadRequest(new { message = "Lūdzu, ievadiet autora vārdu!" });

            var authors = await _db.Authors
                .Include(a => a.Books)
                .Where(a => a.Name.ToLower().Contains(dto.AuthorName.ToLower()))
                .Select(a => new
                {
                    a.Id,
                    a.Name,
                    a.Email,
                    Books = a.Books.Select(b => new { b.Id, b.Title, b.Price })
                })
                .ToListAsync();

            return Ok(authors);
        }

        // ============================================================
        // 2. DAĻA: 3 GET GALAPUNKTI (bez datu ievades)
        // ============================================================

        // GET: api/books/top-expensive
        // 4. TOP 3 dārgākās grāmatas
        [HttpGet("books/top-expensive")]
        public async Task<IActionResult> GetTopExpensiveBooks()
        {
            var topBooks = await _db.Books
                .Include(b => b.Author)
                .OrderByDescending(b => b.Price)
                .Take(3)
                .Select(b => new
                {
                    b.Id,
                    b.Title,
                    b.Price,
                    Author = b.Author != null ? b.Author.Name : null
                })
                .ToListAsync();

            return Ok(topBooks);
        }

        // GET: api/categories/stats
        // 5. Kategoriju kopsavilkums (skaits un vidējā cena)
        [HttpGet("categories/stats")]
        public async Task<IActionResult> GetCategoryStats()
        {
            var stats = await _db.Categories
                .Select(c => new
                {
                    Category = c.Title,
                    BookCount = c.Books.Count,
                    AveragePrice = c.Books.Any() ? Math.Round(c.Books.Average(b => b.Price), 2) : 0
                })
                .ToListAsync();

            return Ok(stats);
        }

        // GET: api/authors/rank
        // 6. Autoru rangs pēc sarakstīto grāmatu skaita
        [HttpGet("authors/rank")]
        public async Task<IActionResult> GetAuthorsRank()
        {
            var rank = await _db.Authors
                .Select(a => new
                {
                    Author = a.Name,
                    BookCount = a.Books.Count
                })
                .OrderByDescending(a => a.BookCount)
                .ThenBy(a => a.Author)
                .ToListAsync();

            return Ok(rank);
        }

        // ============================================================
        // 3. DAĻA: 1 DELETE GALAPUNKTS (datu dzēšanai)
        // ============================================================

        // DELETE: api/books/{id}
        // Dzēš grāmatu pēc tās ID
        [HttpDelete("books/{id:int}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _db.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound(new { message = $"Grāmata ar ID {id} netika atrasta!" });
            }

            _db.Books.Remove(book);
            await _db.SaveChangesAsync();

            return Ok(new { message = $"Grāmata '{book.Title}' (ID: {id}) veiksmīgi izdzēsta!" });
        }
    }
}