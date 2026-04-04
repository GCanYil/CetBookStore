using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using CetBookStore.Data;
using CetBookStore.Models;
using System.Security.Claims;

namespace CetBookStore.Controllers;
[Authorize]
public class OrdersController : Controller
{
    private readonly ApplicationDbContext _context;

    public OrdersController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var orders = await _context.Orders.Include(o => o.OrderItems)
            .Where(o => o.UserId == userId).OrderByDescending(o => o.OrderDate)
            .ToListAsync();
        return View(orders);
    }
}