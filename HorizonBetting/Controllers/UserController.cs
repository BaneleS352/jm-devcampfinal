using HorizonBetting.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HorizonBetting.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Mock User Dashboard. In a real app, the UserId would come from authentication claims.
        public async Task<IActionResult> Dashboard(int? id)
        {
            // If no ID is provided, just pick the first user for demo purposes, or return a list to select a user.
            if (id == null)
            {
                var firstUser = await _context.Users.FirstOrDefaultAsync();
                if (firstUser != null)
                {
                    return RedirectToAction(nameof(Dashboard), new { id = firstUser.Id });
                }
                return View("NoUsers"); // Custom view if no users exist
            }

            var user = await _context.Users
                .Include(u => u.Accounts)
                    .ThenInclude(a => a.Transactions)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return NotFound();

            return View(user);
        }
        
        public IActionResult NoUsers()
        {
            return View();
        }
    }
}
