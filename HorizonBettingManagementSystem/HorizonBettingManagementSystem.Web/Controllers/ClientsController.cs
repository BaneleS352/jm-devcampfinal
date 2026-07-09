using HorizonBettingManagementSystem.Web.Data;
using HorizonBettingManagementSystem.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace HorizonBettingManagementSystem.Web.Controllers;

public class ClientsController : Controller
{
    private readonly ApplicationDbContext _context;

    public ClientsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // READ (LIST)
    public IActionResult Index()
    {
        try
        {
            var clients = _context.Clients
                .Include(c => c.Accounts)
                .ToList();

            ViewBag.Count = clients.Count;

            return View(clients);
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Database error while loading clients: {ex.Message}";
            return View(new List<Client>());
        }
    }

    // DETAILS
    public IActionResult Details(int id)
    {
        var client = _context.Clients
            .Include(c => c.Accounts)
            .FirstOrDefault(c => c.ClientId == id);

        if (client == null)
        {
            TempData["Error"] = $"Client with ID {id} was not found.";
            return RedirectToAction("Index");
        }

        return View(client);
    }

    // CREATE (GET)
    public IActionResult Create()
    {
        return View();
    }

    // CREATE (POST)
    [HttpPost]
    public IActionResult Create(Client client)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Please correct the highlighted fields before submitting.";
            return View(client);
        }

        try
        {
            client.CreatedDate = DateTimeOffset.Now;

            if (_context.Clients.Any(c => c.IdNumber == client.IdNumber))
            {
                ModelState.AddModelError(
                    nameof(client.IdNumber),
                    "A client with this ID Number already exists.");

                return View(client);
            }

            _context.Clients.Add(client);
            _context.SaveChanges();

            TempData["Success"] = $"Client '{client.FirstName} {client.LastName}' created successfully.";

            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Failed to create client. Reason: {ex.Message}";
            return View(client);
        }
    }

    // EDIT (GET)
    public IActionResult Edit(int id)
    {
        var client = _context.Clients.Find(id);

        if (client == null)
        {
            TempData["Error"] = $"Client with ID {id} not found.";
            return RedirectToAction("Index");
        }

        return View(client);
    }

    // EDIT (POST)
    [HttpPost]
    public IActionResult Edit(Client client)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Invalid data. Please review the form.";
            return View(client);
        }

        if (_context.Clients.Any(c =>
            c.IdNumber == client.IdNumber &&
            c.ClientId != client.ClientId))
        {
            ModelState.AddModelError(
                nameof(client.IdNumber),
                "Another client already uses this ID Number.");

            return View(client);
        }

        try
        {
            _context.Clients.Update(client);
            _context.SaveChanges();

            TempData["Success"] = "Client updated successfully.";

            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Update failed. Reason: {ex.Message}";
            return View(client);
        }
    }

    // DELETE (GET)
    public IActionResult Delete(int id)
    {
        var client = _context.Clients.Find(id);

        if (client == null)
        {
            TempData["Error"] = $"Client with ID {id} not found.";
            return RedirectToAction("Index");
        }

        return View(client);
    }

    // DELETE (POST)
    [HttpPost]
    public IActionResult DeleteConfirmed(int id)
    {
        try
        {
            var client = _context.Clients.Find(id);

            if (client == null)
            {
                TempData["Error"] = $"Client with ID {id} does not exist.";
                return RedirectToAction("Index");
            }

            _context.Clients.Remove(client);
            _context.SaveChanges();

            TempData["Success"] = $"Client '{client.FirstName}' deleted successfully.";

            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Deletion failed. Possible reason: related records exist. Details: {ex.Message}";
            return RedirectToAction("Index");
        }
    }
}