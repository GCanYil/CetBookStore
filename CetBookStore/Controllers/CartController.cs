using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using CetBookStore.Data;
using CetBookStore.Models;
using System.Security.Claims;

namespace CetBookStore.Controllers;
[Authorize]
public class CartController : Controller
{
    private readonly ApplicationDbContext _context;

    public CartController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var cartItems = await _context.CartItems
            .Include(c => c.Book)
            .Where(c => c.UserId == userId).ToListAsync();
        return View(cartItems);
    }

    [HttpPost]
    public async Task<IActionResult> AddToCart(int bookId, int quantity = 1)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var book = await _context.Books.FindAsync(bookId);
        if (book == null)
        {
            return NotFound();
        }

        var existingItem = await _context.CartItems.FirstOrDefaultAsync(c => c.BookId == bookId && c.UserId == userId);
        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
        }
        else
        {
            var cartItem = new CartItem
            {
                BookId = bookId,
                UserId = userId,
                Quantity = quantity,
                Price = book.Price
            };
            
            _context.CartItems.Add(cartItem);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Purchase()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var cartItems = await _context.CartItems.Include(c => c.Book)
            .Where(c => c.UserId == userId).ToListAsync();
        if (!cartItems.Any())
        {
            return RedirectToAction("Index");
        }

        var order = new Order
        {
            UserId = userId,
            OrderDate = DateTime.UtcNow,
            TotalPrice = cartItems.Sum(x => x.Quantity * x.Price),
            OrderItems = new List<OrderItem>()
        };

        foreach (var item in cartItems)
        {
            order.OrderItems.Add(new OrderItem
            {
                BookId = item.BookId,
                BookTitle = item.Book.Title,
                Quantity = item.Quantity,
                Price = item.Price
            });
        }

        _context.Orders.Add(order);
        _context.CartItems.RemoveRange(cartItems);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Orders");
    }
}