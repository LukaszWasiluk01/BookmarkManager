using BookmarkManager.Data;
using BookmarkManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookmarkManager.Controllers
{
    [Authorize]
    public class FriendsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public FriendsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var currentUserId = _userManager.GetUserId(User);

            var friendships = await _context.Friendships
                .Include(f => f.Requester)
                .Include(f => f.Addressee)
                .Where(f => (f.RequesterId == currentUserId || f.AddresseeId == currentUserId) && f.IsAccepted)
                .ToListAsync();

            return View(friendships);
        }

        public async Task<IActionResult> Requests()
        {
            var currentUserId = _userManager.GetUserId(User);

            var pendingRequests = await _context.Friendships
                .Include(f => f.Requester)
                .Where(f => f.AddresseeId == currentUserId && !f.IsAccepted)
                .ToListAsync();

            return View(pendingRequests);
        }

        [HttpPost]
        public async Task<IActionResult> SendRequest(string addresseeEmail)
        {
            var currentUserId = _userManager.GetUserId(User);
            var addressee = await _userManager.FindByEmailAsync(addresseeEmail);

            if (addressee == null || addressee.Id == currentUserId)
            {
                TempData["Error"] = "Nie znaleziono użytkownika lub próbujesz dodać siebie.";
                return RedirectToAction(nameof(Index));
            }

            var existingFriendship = await _context.Friendships
                .FirstOrDefaultAsync(f =>
                    (f.RequesterId == currentUserId && f.AddresseeId == addressee.Id) ||
                    (f.RequesterId == addressee.Id && f.AddresseeId == currentUserId));

            if (existingFriendship != null)
            {
                TempData["Error"] = "Zaproszenie już istnieje lub jesteście znajomymi.";
                return RedirectToAction(nameof(Index));
            }

            var friendship = new Friendship
            {
                RequesterId = currentUserId,
                AddresseeId = addressee.Id,
                IsAccepted = false
            };

            _context.Friendships.Add(friendship);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Wysłano zaproszenie.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> AcceptRequest(int id)
        {
            var currentUserId = _userManager.GetUserId(User);
            var friendship = await _context.Friendships.FirstOrDefaultAsync(f => f.Id == id && f.AddresseeId == currentUserId);

            if (friendship != null)
            {
                friendship.IsAccepted = true;
                _context.Update(friendship);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Requests));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFriend(int id)
        {
            var currentUserId = _userManager.GetUserId(User);
            var friendship = await _context.Friendships.FirstOrDefaultAsync(f =>
                f.Id == id && (f.RequesterId == currentUserId || f.AddresseeId == currentUserId));

            if (friendship != null)
            {
                _context.Friendships.Remove(friendship);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}