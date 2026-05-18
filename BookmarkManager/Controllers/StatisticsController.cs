using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookmarkManager.Data;
using BookmarkManager.Models;

namespace BookmarkManager.Controllers
{
    public class StatisticsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StatisticsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var totalUsers = await _context.Users.CountAsync();
            var totalBookmarks = await _context.Bookmarks.CountAsync();
            var totalCategories = await _context.Categories.CountAsync();

            var mostPopularCategory = await _context.Bookmarks
                .Include(b => b.Category)
                .GroupBy(b => b.Category.Name)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefaultAsync();

            var mostActiveUser = await _context.Bookmarks
                .Include(b => b.User)
                .GroupBy(b => b.User.Email)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefaultAsync();

            var viewModel = new StatisticsViewModel
            {
                TotalUsers = totalUsers,
                TotalBookmarks = totalBookmarks,
                TotalCategories = totalCategories,
                MostPopularCategory = mostPopularCategory ?? "Brak danych",
                MostActiveUser = mostActiveUser ?? "Brak danych"
            };

            return View(viewModel);
        }
    }
}