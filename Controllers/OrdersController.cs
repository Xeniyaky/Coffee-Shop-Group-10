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

        // temporary in-memory storage
        private static List<Order> orders = new List<Order>();

        // READ (View all orders)
        public async Task<IActionResult> Index()
        {
            // The .Include(o => o.OrderItems) is mandatory!
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
        public IActionResult Create(string CustomerName, string[] DrinkItems, string[] Sizes, int[] Quantities)
        {
            // 1. Validation: Ensure we actually have items to add
            if (DrinkItems == null || DrinkItems.Length == 0)
            {
                ModelState.AddModelError("", "You must add at least one item to the order.");
                return View();
            }

            // 2. Create the Master Order
            var newOrder = new Order
            {
                CustomerName = CustomerName,
                OrderDate = DateTime.Now,
                OrderItems = new List<OrderItem>(),
                Total = 0 // We will calculate this in the loop
            };

            decimal grandTotal = 0;

            // 3. Loop through the arrays and add OrderItems
            for (int i = 0; i < DrinkItems.Length; i++)
            {
                // Skip any empty rows if the user clicked "Add" but didn't type anything
                if (string.IsNullOrWhiteSpace(DrinkItems[i])) continue;

                var item = new OrderItem
                {
                    DrinkName = DrinkItems[i],
                    Size = Sizes[i],
                    Quantity = Quantities[i]
                };

                // Simple Pricing Logic: $5 per drink (adjust as needed)
                grandTotal += (Quantities[i] * 5.00m);

                newOrder.OrderItems.Add(item);
            }

            newOrder.Total = grandTotal;

            // 4. Save to Database
            _context.Orders.Add(newOrder);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: Edit page
        public IActionResult Edit(int id)
        {
            var order = orders.FirstOrDefault(o => o.Id == id);
            return View(order);
        }

        // POST: Edit order
        [HttpPost]
        public IActionResult Edit(Order updatedOrder)
        {
            // 1. Pull the existing order from the database, including its items
            var order = _context.Orders
                                .Include(o => o.OrderItems)
                                .FirstOrDefault(o => o.Id == updatedOrder.Id);

            if (order != null)
            {
                // 2. Update the master info
                order.CustomerName = updatedOrder.CustomerName;

                // Note: For a multi-item order, you usually edit items in a separate logic
                // For now, we update the Total if it was changed
                order.Total = updatedOrder.Total;

                // 3. Save to the database
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        // GET: Delete page
        public async Task<IActionResult> Delete(int? id)
        {
            // You MUST include OrderItems here or the list will be empty on the delete page
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(m => m.Id == id);

            return View(order);
        }

        // POST: Delete order
        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            var order = orders.FirstOrDefault(o => o.Id == id);

            if (order != null)
            {
                orders.Remove(order);
            }

            return RedirectToAction("Index");
        }
    }
}