using EfWebDatabaseApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EfWebDatabaseApp.Controllers
{
    public class QueriesController : Controller
    {
        private readonly AppDbContext _db;

        public QueriesController(AppDbContext db)
        {
            _db = db;
        }

        // Galvenā izvēlne ar saitēm uz visiem 6 vaicājumiem
        public IActionResult Index() => View();

        // 1. Vaicājums [IEVADE]: Meklēšana pēc nosaukuma fragmenta
        public IActionResult Query1(string? keyword)
        {
            ViewBag.Keyword = keyword;
            var query = _db.Books.Include(b => b.Author).AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(b => b.Title.ToLower().Contains(keyword.ToLower()));
            }

            return View(query.ToList());
        }

        // 2. Vaicājums [IEVADE]: Grāmatas līdz noteiktai cenai
        public IActionResult Query2(decimal? maxPrice)
        {
            ViewBag.MaxPrice = maxPrice;
            var query = _db.Books.Include(b => b.Author).Include(b => b.Category).AsQueryable();

            if (maxPrice.HasValue)
            {
                query = query.Where(b => b.Price <= maxPrice.Value).OrderBy(b => b.Price);
            }

            return View(query.ToList());
        }

        // 3. Vaicājums [IEVADE]: Meklēt autoru un redzēt viņa darbus
        public IActionResult Query3(string? authorName)
        {
            ViewBag.AuthorName = authorName;
            var query = _db.Authors.Include(a => a.Books).AsQueryable();

            if (!string.IsNullOrWhiteSpace(authorName))
            {
                query = query.Where(a => a.Name.ToLower().Contains(authorName.ToLower()));
            }

            return View(query.ToList());
        }

        // 4. Vaicājums [AUTO]: TOP 3 dārgākās grāmatas
        public IActionResult Query4()
        {
            var topBooks = _db.Books
                .Include(b => b.Author)
                .OrderByDescending(b => b.Price)
                .Take(3)
                .ToList();

            return View(topBooks);
        }

        // 5. Vaicājums [AUTO]: Kategoriju kopsavilkums (statistika)
        public IActionResult Query5()
        {
            var stats = _db.Categories
                .Select(c => new CategoryStatViewModel
                {
                    CategoryTitle = c.Title,
                    BookCount = c.Books.Count,
                    AveragePrice = c.Books.Any() ? c.Books.Average(b => b.Price) : 0
                })
                .ToList();

            return View(stats);
        }

        // 6. Vaicājums [AUTO]: Autoru rangs pēc grāmatu skaita
        public IActionResult Query6()
        {
            var authorsRank = _db.Authors
                .Select(a => new AuthorRankViewModel
                {
                    AuthorName = a.Name,
                    BookCount = a.Books.Count
                })
                .OrderByDescending(a => a.BookCount)
                .ThenBy(a => a.AuthorName)
                .ToList();

            return View(authorsRank);
        }
    }

    // Papildu datu modeļi statistikas skatiem
    public class CategoryStatViewModel
    {
        public string CategoryTitle { get; set; } = string.Empty;
        public int BookCount { get; set; }
        public decimal AveragePrice { get; set; }
    }

    public class AuthorRankViewModel
    {
        public string AuthorName { get; set; } = string.Empty;
        public int BookCount { get; set; }
    }
}