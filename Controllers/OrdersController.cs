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
            // Clean out any empty items the user might have left
            order.OrderItems.RemoveAll(i => string.IsNullOrEmpty(i.DrinkName));

            if (order.OrderItems.Any())
            {
                order.OrderDate = DateTime.Now;

                // FORCED CALCULATION: This ignores any previous totals and calculates fresh
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

            // Mandatory: .Include(o => o.OrderItems) so the items load into the Edit fields
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

                // 3. DATABASE SYNC: 
                // Remove the old items from the database first
                _context.OrderItems.RemoveRange(existingOrder.OrderItems);

                // 4. PRICING LOGIC:
                // Calculate the fresh total based on the new prices and quantities from the form
                if (order.OrderItems != null && order.OrderItems.Any())
                {
                    // Filter out any blank rows the user might have added accidentally
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
                await _context.SaveChangesAsync(); // Fixed: Added await and Async
            }
            return RedirectToAction(nameof(Index));
        }
    }
}