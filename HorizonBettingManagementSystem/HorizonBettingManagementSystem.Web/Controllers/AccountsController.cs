using HorizonBettingManagementSystem.Web.Data;
using HorizonBettingManagementSystem.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace HorizonBettingManagementSystem.Web.Controllers
{
    public class AccountsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Details(int id)
        {
            var account = _context.Accounts
                .Include(a => a.Client)
                .Include(a => a.Transactions)
                .FirstOrDefault(a => a.AccountId == id);

            if (account == null)
            {
                TempData["Error"] = $"Account with ID {id} not found.";
                return RedirectToAction("Index", "Clients");
            }

            return View(account);
        }

        public IActionResult Create(int clientId)
        {
            ViewBag.ClientId = clientId;
            return View();
        }

        [HttpPost]
        public IActionResult Create(Account account)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Invalid account data.";
                return View(account);
            }

            var clientExists = _context.Clients.Any(c => c.ClientId == account.ClientId);

            if (!clientExists)
            {
                ModelState.AddModelError(
                    "",
                    "The selected client does not exist.");

                return View(account);
            }

            try
            {
                account.CreatedDate = DateTimeOffset.Now;

                _context.Accounts.Add(account);
                _context.SaveChanges();

                TempData["Success"] = "Account created successfully.";

                return RedirectToAction("Details", "Clients", new { id = account.ClientId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Failed to create account: {ex.Message}";
                return View(account);
            }
        }

        public IActionResult Edit(int id)
        {
            var account = _context.Accounts.Find(id);

            if (account == null)
            {
                TempData["Error"] = "Account not found.";
                return RedirectToAction("Index", "Clients");
            }

            return View(account);
        }

        [HttpPost]
        public IActionResult Edit(Account account)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Invalid account data.";
                return View(account);
            }

            try
            {
                _context.Accounts.Update(account);
                _context.SaveChanges();

                TempData["Success"] = "Account updated successfully.";

                return RedirectToAction("Details", new { id = account.AccountId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Update failed: {ex.Message}";
                return View(account);
            }
        }

        public IActionResult Delete(int id)
        {
            var account = _context.Accounts.Find(id);

            if (account == null)
            {
                TempData["Error"] = "Account not found.";
                return RedirectToAction("Index", "Clients");
            }

            return View(account);
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                var account = _context.Accounts.Find(id);

                if (account == null)
                {
                    TempData["Error"] = "Account not found.";
                    return RedirectToAction("Index", "Clients");
                }

                _context.Accounts.Remove(account);
                _context.SaveChanges();

                TempData["Success"] = "Account deleted successfully.";

                return RedirectToAction("Index", "Clients");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Delete failed. Likely due to linked transactions. Details: {ex.Message}";
                return RedirectToAction("Index", "Clients");
            }
        }
    }
}