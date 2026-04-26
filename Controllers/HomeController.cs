using CoffeeShopMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text.Json;

namespace CoffeeShopMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;

        private readonly Data.ApplicationDbContext _context;

        public HomeController(IConfiguration configuration, Data.ApplicationDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public async Task<IActionResult> Charts(List<string> productFilter, decimal? minPrice, DateTime? startDate, DateTime? endDate)
        {
            // 1. Get the dynamic checklist items from the database
            ViewBag.AllProducts = await _context.OrderItems
                .Select(i => i.DrinkName)
                .Distinct()
                .ToListAsync();

            // 2. Filter the Orders (Receipts)
            var query = _context.Orders.Include(o => o.OrderItems).AsQueryable();

            if (productFilter != null && productFilter.Count > 0)
            {
                // Filters for orders that contain at least one of the selected products
                query = query.Where(o => o.OrderItems.Any(i => productFilter.Contains(i.DrinkName)));
            }

            if (minPrice.HasValue)
            {
                query = query.Where(o => o.Total >= minPrice.Value);
            }

            if (startDate.HasValue)
            {
                query = query.Where(o => o.OrderDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(o => o.OrderDate <= endDate.Value);
            }

            var filteredOrders = await query.ToListAsync();

            // --- DATA FOR CHART 1: Sales Over Time ---
            var salesOverTime = filteredOrders
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new {
                    Date = g.Key.ToShortDateString(),
                    Total = g.Sum(o => o.Total)
                })
                .OrderBy(g => DateTime.Parse(g.Date))
                .ToList();

            // --- DATA FOR CHART 2: Product Popularity ---
            // All items from selected orders
            var itemsForChart = filteredOrders.SelectMany(o => o.OrderItems).AsQueryable();

            // Filter out unselected items, double-check
            if (productFilter != null && productFilter.Count > 0)
            {
                itemsForChart = itemsForChart.Where(i => productFilter.Contains(i.DrinkName));
            }

            var popularDrinks = itemsForChart
                .GroupBy(i => i.DrinkName)
                .Select(g => new {
                    Name = g.Key,
                    Count = g.Sum(i => i.Quantity)
                })
                .OrderByDescending(g => g.Count)
                .ToList();

            // Pass everything to the View
            ViewBag.SalesLabels = salesOverTime.Select(s => s.Date).ToList();
            ViewBag.SalesValues = salesOverTime.Select(s => s.Total).ToList();
            ViewBag.DrinkLabels = popularDrinks.Select(p => p.Name).ToList();
            ViewBag.DrinkValues = popularDrinks.Select(p => p.Count).ToList();

            return View();
        }

        public async Task<IActionResult> Nutrition(string query)
        {
            var nutritionItems = new List<NutritionItem>();

            if (!string.IsNullOrWhiteSpace(query))
            {
                var apiKey = _configuration["ApiSettings:ApiKey"];
                var url = $"https://api.api-ninjas.com/v1/nutrition?query={Uri.EscapeDataString(query)}";

                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("X-Api-Key", apiKey);

                var response = await client.GetAsync(url);
                var json = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode && json.Trim().StartsWith("["))
                {
                    using var document = JsonDocument.Parse(json);
                    foreach (var item in document.RootElement.EnumerateArray())
                    {
                        nutritionItems.Add(new NutritionItem
                        {
                            Name = GetValue(item, "name"),
                            ServingSizeG = GetValue(item, "serving_size_g"),
                            FatTotalG = GetValue(item, "fat_total_g"),
                            CarbohydratesTotalG = GetValue(item, "carbohydrates_total_g"),
                            SugarG = GetValue(item, "sugar_g")
                        });
                    }
                }
            }

            ViewBag.Query = query;
            return View(nutritionItems);
        }

        private string GetValue(JsonElement item, string propertyName)
        {
            if (item.TryGetProperty(propertyName, out var value))
            {
                return value.ToString();
            }

            return "";
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}