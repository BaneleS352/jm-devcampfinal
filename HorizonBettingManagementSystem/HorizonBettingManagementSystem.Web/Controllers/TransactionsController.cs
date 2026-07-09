using HorizonBettingManagementSystem.Web.Data;
using HorizonBettingManagementSystem.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace HorizonBettingManagementSystem.Web.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TransactionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Create(int accountId)
        {
            ViewBag.AccountId = accountId;
            return View();
        }

        [HttpPost]
        public IActionResult Create(Transaction transaction)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Invalid transaction data.";
                return View(transaction);
            }

            var accountExists = _context.Accounts.Any(a => a.AccountId == transaction.AccountId);

            if (!accountExists)
            {
                ModelState.AddModelError(
                    "",
                    "The selected account does not exist.");

                return View(transaction);
            }

            var validTypes = new[]
            {
                "Credit",
                "Debit",
                "Voucher"
            };

            if (!validTypes.Contains(transaction.TransactionType))
            {
                ModelState.AddModelError(
                    nameof(transaction.TransactionType),
                    "Transaction type must be Credit, Debit or Voucher.");

                return View(transaction);
            }

            try
            {
                transaction.CaptureDate = DateTimeOffset.Now;

                _context.Transactions.Add(transaction);
                _context.SaveChanges();

                TempData["Success"] = "Transaction recorded successfully.";

                return RedirectToAction("Details", "Accounts", new { id = transaction.AccountId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Transaction failed: {ex.Message}";
                return View(transaction);
            }
        }

        public IActionResult Edit(int id)
        {
            var transaction = _context.Transactions.Find(id);

            if (transaction == null)
            {
                TempData["Error"] = "Transaction not found.";
                return RedirectToAction("Index", "Clients");
            }

            return View(transaction);
        }

        [HttpPost]
        public IActionResult Edit(Transaction transaction)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Invalid transaction data.";
                return View(transaction);
            }

            var accountExists = _context.Accounts.Any(a => a.AccountId == transaction.AccountId);

            if (!accountExists)
            {
                ModelState.AddModelError(
                    "",
                    "The selected account does not exist.");

                return View(transaction);
            }

            var validTypes = new[]
            {
                "Credit",
                "Debit",
                "Voucher"
            };

            if (!validTypes.Contains(transaction.TransactionType))
            {
                ModelState.AddModelError(
                    nameof(transaction.TransactionType),
                    "Transaction type must be Credit, Debit or Voucher.");

                return View(transaction);
            }

            try
            {
                transaction.CaptureDate = DateTimeOffset.Now;

                _context.Transactions.Update(transaction);
                _context.SaveChanges();

                TempData["Success"] = "Transaction updated successfully.";

                return RedirectToAction("Details", "Accounts", new { id = transaction.AccountId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Update failed: {ex.Message}";
                return View(transaction);
            }
        }

        public IActionResult Delete(int id)
        {
            var transaction = _context.Transactions.Find(id);

            if (transaction == null)
            {
                TempData["Error"] = "Transaction not found.";
                return RedirectToAction("Index", "Clients");
            }

            return View(transaction);
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                var transaction = _context.Transactions.Find(id);

                if (transaction == null)
                {
                    TempData["Error"] = "Transaction not found.";
                    return RedirectToAction("Index", "Clients");
                }

                _context.Transactions.Remove(transaction);
                _context.SaveChanges();

                TempData["Success"] = "Transaction deleted successfully.";

                return RedirectToAction("Details", "Accounts", new { id = transaction.AccountId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Delete failed: {ex.Message}";
                return RedirectToAction("Index", "Clients");
            }
        }
    }
}