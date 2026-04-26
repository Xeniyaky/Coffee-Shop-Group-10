using CoffeeShopMVC.Data;
using CoffeeShopMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShopMVC.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // READ: View all orders
        public async Task<IActionResult> Index()
        {
            var orders = await _context.Orders
                                       .Include(o => o.OrderItems)
                                       .ToListAsync();
            return View(orders);
        }

        // GET: Create page
        public IActionResult Create()
        {
            return View();
        }

        // POST: Create order
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Order order)
        {
            // Clean out empty items 
            order.OrderItems.RemoveAll(i => string.IsNullOrEmpty(i.DrinkName));

            if (order.OrderItems.Any())
            {
                order.OrderDate = DateTime.Now;

                order.Total = order.OrderItems.Sum(item => item.Quantity * item.Price);

                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        // GET: Edit page
        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (order == null) return NotFound();

            return View(order);
        }

        // POST: Orders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Order order)
        {
            if (id != order.Id) return NotFound();

            try
            {
                // 1. Fetch the existing order from the DB including its current items
                var existingOrder = await _context.Orders
                    .Include(o => o.OrderItems)
                    .FirstOrDefaultAsync(o => o.Id == id);

                if (existingOrder == null) return NotFound();

                // 2. Update the top-level info
                existingOrder.CustomerName = order.CustomerName;

                // 3. DATABASE SYNC: Remove old items
                _context.OrderItems.RemoveRange(existingOrder.OrderItems);

                // 4. PRICING LOGIC: Prices * Quantity
                if (order.OrderItems != null && order.OrderItems.Any())
                {
                    var validItems = order.OrderItems.Where(i => !string.IsNullOrWhiteSpace(i.DrinkName)).ToList();

                    existingOrder.OrderItems = validItems;
                    existingOrder.Total = validItems.Sum(i => i.Quantity * i.Price);
                }
                else
                {
                    existingOrder.Total = 0;
                }

                // 5. Save everything
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Orders.Any(e => e.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // GET: Delete page
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (order == null) return NotFound();
            return View(order);
        }

        // POST: Delete order
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.Id == id);
            if (order != null)
            {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync(); 
            }
            return RedirectToAction(nameof(Index));
        }
    }
}