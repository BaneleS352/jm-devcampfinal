using HorizonBetting.Data;
using HorizonBetting.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HorizonBetting.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- USER MANAGEMENT ---

        public async Task<IActionResult> UserList(string searchString)
        {
            var users = from u in _context.Users select u;

            if (!string.IsNullOrEmpty(searchString))
            {
                users = users.Where(u => u.IdNumber.Contains(searchString) || u.Surname.Contains(searchString));
            }

            return View(await users.ToListAsync());
        }

        public IActionResult CreateUser()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser([Bind("IdNumber,FirstName,Surname,Email")] User user)
        {
            if (ModelState.IsValid)
            {
                if (await _context.Users.AnyAsync(u => u.IdNumber == user.IdNumber))
                {
                    ModelState.AddModelError("IdNumber", "A user with this ID number already exists.");
                    return View(user);
                }

                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(UserList));
            }
            return View(user);
        }

        public async Task<IActionResult> EditUser(int? id)
        {
            if (id == null) return NotFound();

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(int id, [Bind("Id,IdNumber,FirstName,Surname,Email")] User user)
        {
            if (id != user.Id) return NotFound();

            if (ModelState.IsValid)
            {
                if (await _context.Users.AnyAsync(u => u.IdNumber == user.IdNumber && u.Id != user.Id))
                {
                    ModelState.AddModelError("IdNumber", "A user with this ID number already exists.");
                    return View(user);
                }

                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(UserList));
            }
            return View(user);
        }

        public async Task<IActionResult> UserDetails(int? id)
        {
            if (id == null) return NotFound();

            var user = await _context.Users
                .Include(u => u.Accounts)
                .FirstOrDefaultAsync(m => m.Id == id);
                
            if (user == null) return NotFound();

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.Include(u => u.Accounts).FirstOrDefaultAsync(u => u.Id == id);
            if (user != null)
            {
                if (user.Accounts.Any(a => a.OutstandingBalance != 0))
                {
                    TempData["ErrorMessage"] = "Cannot delete a user with active accounts or non-zero balances.";
                    return RedirectToAction(nameof(UserList));
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(UserList));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

        // --- ACCOUNT MANAGEMENT ---

        public IActionResult CreateAccount(int userId)
        {
            var account = new Account { UserId = userId };
            return View(account);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAccount([Bind("AccountNumber,UserId")] Account account)
        {
            // OutstandingBalance should default to 0
            if (ModelState.IsValid)
            {
                if (await _context.Accounts.AnyAsync(a => a.AccountNumber == account.AccountNumber))
                {
                    ModelState.AddModelError("AccountNumber", "This account number already exists.");
                    return View(account);
                }

                _context.Add(account);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(UserDetails), new { id = account.UserId });
            }
            return View(account);
        }

        public async Task<IActionResult> AccountDetails(int? id)
        {
            if (id == null) return NotFound();

            var account = await _context.Accounts
                .Include(a => a.User)
                .Include(a => a.Transactions)
                .FirstOrDefaultAsync(m => m.Id == id);
                
            if (account == null) return NotFound();

            return View(account);
        }

        // --- TRANSACTION MANAGEMENT ---

        public IActionResult CreateTransaction(int accountId)
        {
            var transaction = new Transaction 
            { 
                AccountId = accountId,
                TransactionDate = DateTime.Today
            };
            return View(transaction);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTransaction([Bind("TransactionDate,Amount,Type,AccountId")] Transaction transaction)
        {
            if (transaction.TransactionDate > DateTime.Today)
            {
                ModelState.AddModelError("TransactionDate", "Transaction date cannot be in the future.");
            }

            if (transaction.Amount <= 0)
            {
                ModelState.AddModelError("Amount", "Amount must be greater than zero.");
            }

            if (ModelState.IsValid)
            {
                transaction.CaptureDate = DateTime.Now;
                _context.Add(transaction);

                // Update account balance
                var account = await _context.Accounts.FindAsync(transaction.AccountId);
                if (account != null)
                {
                    if (transaction.Type == TransactionType.Debit)
                    {
                        account.OutstandingBalance -= transaction.Amount;
                    }
                    else
                    {
                        account.OutstandingBalance += transaction.Amount;
                    }
                    _context.Update(account);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(AccountDetails), new { id = transaction.AccountId });
            }
            return View(transaction);
        }

        public async Task<IActionResult> EditTransaction(int? id)
        {
            if (id == null) return NotFound();

            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null) return NotFound();
            return View(transaction);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTransaction(int id, [Bind("Id,TransactionDate,Amount,Type,AccountId")] Transaction transaction)
        {
            if (id != transaction.Id) return NotFound();

            if (transaction.TransactionDate > DateTime.Today)
            {
                ModelState.AddModelError("TransactionDate", "Transaction date cannot be in the future.");
            }

            if (transaction.Amount <= 0)
            {
                ModelState.AddModelError("Amount", "Amount must be greater than zero.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Need to recalculate balance. Better to pull the old one.
                    var oldTransaction = await _context.Transactions.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
                    if (oldTransaction == null) return NotFound();

                    var account = await _context.Accounts.FindAsync(transaction.AccountId);
                    if (account != null)
                    {
                        // Reverse old
                        if (oldTransaction.Type == TransactionType.Debit) account.OutstandingBalance += oldTransaction.Amount;
                        else account.OutstandingBalance -= oldTransaction.Amount;

                        // Apply new
                        if (transaction.Type == TransactionType.Debit) account.OutstandingBalance -= transaction.Amount;
                        else account.OutstandingBalance += transaction.Amount;
                        
                        _context.Update(account);
                    }

                    transaction.CaptureDate = DateTime.Now; // Update capture date as per business rules
                    _context.Update(transaction);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransactionExists(transaction.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(AccountDetails), new { id = transaction.AccountId });
            }
            return View(transaction);
        }

        private bool TransactionExists(int id)
        {
            return _context.Transactions.Any(e => e.Id == id);
        }
    }
}
