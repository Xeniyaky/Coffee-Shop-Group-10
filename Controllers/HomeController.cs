using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using CoffeeShopMVC.Models;

namespace CoffeeShopMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;

        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Charts()
        {
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