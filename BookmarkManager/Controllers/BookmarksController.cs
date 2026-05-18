using BookmarkManager.Data;
using BookmarkManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList.Extensions;

namespace BookmarkManager.Controllers
{
    [Authorize]
    public class BookmarksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public BookmarksController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Bookmarks
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? categoryId, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.DateSortParm = String.IsNullOrEmpty(sortOrder) ? "date_desc" : "";
            ViewBag.DescSortParm = sortOrder == "desc" ? "desc_desc" : "desc";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentCategory = categoryId;

            var userId = _userManager.GetUserId(User);
            var bookmarks = _context.Bookmarks
                .Include(b => b.Category)
                .Include(b => b.User)
                .Where(b => b.UserId == userId);

            if (categoryId.HasValue)
            {
                bookmarks = bookmarks.Where(b => b.CategoryId == categoryId.Value);
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                bookmarks = bookmarks.Where(b => b.Description.Contains(searchString) || b.Url.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "date_desc":
                    bookmarks = bookmarks.OrderByDescending(b => b.CreatedAt);
                    break;
                case "desc":
                    bookmarks = bookmarks.OrderBy(b => b.Description);
                    break;
                case "desc_desc":
                    bookmarks = bookmarks.OrderByDescending(b => b.Description);
                    break;
                default:
                    bookmarks = bookmarks.OrderBy(b => b.CreatedAt);
                    break;
            }

            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", categoryId);

            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(bookmarks.ToPagedList(pageNumber, pageSize));
        }

        // GET: Bookmarks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            var bookmark = await _context.Bookmarks
                .Include(b => b.Category)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);
            if (bookmark == null)
            {
                return NotFound();
            }

            return View(bookmark);
        }

        // GET: Bookmarks/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: Bookmarks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Url,Description,CategoryId")] Bookmark bookmark)
        {
            ModelState.Remove("UserId");
            ModelState.Remove("User");
            ModelState.Remove("Category");

            if (ModelState.IsValid)
            {
                bookmark.UserId = _userManager.GetUserId(User);
                bookmark.CreatedAt = DateTime.Now;

                _context.Add(bookmark);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", bookmark.CategoryId);
            return View(bookmark);
        }

        // GET: Bookmarks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            var bookmark = await _context.Bookmarks.FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);
            if (bookmark == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", bookmark.CategoryId);
            return View(bookmark);
        }

        // POST: Bookmarks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Url,Description,CategoryId")] Bookmark bookmark)
        {
            if (id != bookmark.Id)
            {
                return NotFound();
            }

            ModelState.Remove("UserId");
            ModelState.Remove("User");
            ModelState.Remove("Category");

            if (ModelState.IsValid)
            {
                try
                {
                    var userId = _userManager.GetUserId(User);
                    var existingBookmark = await _context.Bookmarks.FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

                    if (existingBookmark == null)
                    {
                        return NotFound();
                    }

                    existingBookmark.Url = bookmark.Url;
                    existingBookmark.Description = bookmark.Description;
                    existingBookmark.CategoryId = bookmark.CategoryId;

                    _context.Update(existingBookmark);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookmarkExists(bookmark.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", bookmark.CategoryId);
            return View(bookmark);
        }

        // GET: Bookmarks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            var bookmark = await _context.Bookmarks
                .Include(b => b.Category)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);
            if (bookmark == null)
            {
                return NotFound();
            }

            return View(bookmark);
        }

        // POST: Bookmarks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = _userManager.GetUserId(User);
            var bookmark = await _context.Bookmarks.FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);
            if (bookmark != null)
            {
                _context.Bookmarks.Remove(bookmark);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> FriendsFeed(string sortOrder, string currentFilter, string searchString, int? categoryId, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.DateSortParm = String.IsNullOrEmpty(sortOrder) ? "date_desc" : "";
            ViewBag.DescSortParm = sortOrder == "desc" ? "desc_desc" : "desc";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentCategory = categoryId;

            var currentUserId = _userManager.GetUserId(User);

            var friendIds = await _context.Friendships
                .Where(f => (f.RequesterId == currentUserId || f.AddresseeId == currentUserId) && f.IsAccepted)
                .Select(f => f.RequesterId == currentUserId ? f.AddresseeId : f.RequesterId)
                .ToListAsync();

            var bookmarks = _context.Bookmarks
                .Include(b => b.Category)
                .Include(b => b.User)
                .Where(b => friendIds.Contains(b.UserId));

            if (categoryId.HasValue)
            {
                bookmarks = bookmarks.Where(b => b.CategoryId == categoryId.Value);
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                bookmarks = bookmarks.Where(b => b.Description.Contains(searchString) || b.Url.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "date_desc":
                    bookmarks = bookmarks.OrderByDescending(b => b.CreatedAt);
                    break;
                case "desc":
                    bookmarks = bookmarks.OrderBy(b => b.Description);
                    break;
                case "desc_desc":
                    bookmarks = bookmarks.OrderByDescending(b => b.Description);
                    break;
                default:
                    bookmarks = bookmarks.OrderByDescending(b => b.CreatedAt);
                    break;
            }

            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", categoryId);

            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(bookmarks.ToPagedList(pageNumber, pageSize));
        }

        public async Task<IActionResult> FriendBookmarks(string id, string sortOrder, string currentFilter, string searchString, int? categoryId, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.DateSortParm = String.IsNullOrEmpty(sortOrder) ? "date_desc" : "";
            ViewBag.DescSortParm = sortOrder == "desc" ? "desc_desc" : "desc";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentCategory = categoryId;
            ViewBag.FriendId = id;

            var currentUserId = _userManager.GetUserId(User);

            var isFriend = await _context.Friendships
                .AnyAsync(f => ((f.RequesterId == currentUserId && f.AddresseeId == id) || (f.RequesterId == id && f.AddresseeId == currentUserId)) && f.IsAccepted);

            if (!isFriend)
            {
                return NotFound();
            }

            var friendUser = await _userManager.FindByIdAsync(id);
            ViewBag.FriendEmail = friendUser?.Email;

            var bookmarks = _context.Bookmarks
                .Include(b => b.Category)
                .Include(b => b.User)
                .Where(b => b.UserId == id);

            if (categoryId.HasValue)
            {
                bookmarks = bookmarks.Where(b => b.CategoryId == categoryId.Value);
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                bookmarks = bookmarks.Where(b => b.Description.Contains(searchString) || b.Url.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "date_desc":
                    bookmarks = bookmarks.OrderByDescending(b => b.CreatedAt);
                    break;
                case "desc":
                    bookmarks = bookmarks.OrderBy(b => b.Description);
                    break;
                case "desc_desc":
                    bookmarks = bookmarks.OrderByDescending(b => b.Description);
                    break;
                default:
                    bookmarks = bookmarks.OrderByDescending(b => b.CreatedAt);
                    break;
            }

            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", categoryId);

            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(bookmarks.ToPagedList(pageNumber, pageSize));
        }

        private bool BookmarkExists(int id)
        {
            return _context.Bookmarks.Any(e => e.Id == id);
        }
    }
}