using Microsoft.AspNetCore.Mvc;
using CoffeeShopMVC.Models;

namespace CoffeeShopMVC.Controllers
{
    public class OrdersController : Controller
    {
        // temporary in-memory storage
        private static List<Order> orders = new List<Order>();

        // READ (View all orders)
        public IActionResult Index()
        {
            return View(orders);
        }

        // GET: Create page
        public IActionResult Create()
        {
            return View();
        }

        // POST: Create order
        [HttpPost]
        public IActionResult Create(Order order)
        {
            order.Id = orders.Count + 1;
            order.Total = order.Quantity * 5; // simple pricing

            orders.Add(order);

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
            var order = orders.FirstOrDefault(o => o.Id == updatedOrder.Id);

            if (order != null)
            {
                order.CustomerName = updatedOrder.CustomerName;
                order.DrinkItem = updatedOrder.DrinkItem;
                order.Size = updatedOrder.Size;
                order.Quantity = updatedOrder.Quantity;
                order.Total = updatedOrder.Quantity * 5;
            }

            return RedirectToAction("Index");
        }

        // GET: Delete page
        public IActionResult Delete(int id)
        {
            var order = orders.FirstOrDefault(o => o.Id == id);
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