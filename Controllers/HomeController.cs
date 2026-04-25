using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using CoffeeShopMVC.Models;

namespace CoffeeShopMVC.Controllers
{
    public class HomeController : Controller
    {
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
                var apiKey = "qJjwWaC9tV7wGlMrr9HkQYyiYeFmD4YXlYWDOVlb";
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
                            Name = item.GetProperty("name").ToString(),
                            Calories = item.GetProperty("calories").ToString(),
                            ServingSizeG = item.GetProperty("serving_size_g").ToString(),
                            FatTotalG = item.GetProperty("fat_total_g").ToString(),
                            ProteinG = item.GetProperty("protein_g").ToString(),
                            CarbohydratesTotalG = item.GetProperty("carbohydrates_total_g").ToString(),
                            SugarG = item.GetProperty("sugar_g").ToString()
                        });
                    }
                }
                else
                {
                    ViewBag.Error = json;
                }
            }

            ViewBag.Query = query;
            return View(nutritionItems);
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