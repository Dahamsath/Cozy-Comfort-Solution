using CozyComfort.Client.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CozyComfort.Client.Controllers
{
    public class DistributorController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;

        public DistributorController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("UserRole") != "Distributor")
            {
                return RedirectToAction("Login", "Account");
            }

            var client = _clientFactory.CreateClient("CozyComfortAPI");
            var response = await client.GetAsync("orders");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var orders = JsonSerializer.Deserialize<List<Order>>(data, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                
                var distributorOrders = orders.Where(o => o.Status == "Pending" || o.Status == "Approved").ToList();

                return View(distributorOrders);
            }
            return View(new List<Order>());
        }

        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            if (HttpContext.Session.GetString("UserRole") != "Distributor") return RedirectToAction("Login", "Account");

            var client = _clientFactory.CreateClient("CozyComfortAPI");
            await client.PutAsync($"orders/{id}/approve", null);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Ship(int id)
        {
            if (HttpContext.Session.GetString("UserRole") != "Distributor") return RedirectToAction("Login", "Account");

            var client = _clientFactory.CreateClient("CozyComfortAPI");
            await client.PutAsync($"orders/{id}/ship", null);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult AddBlanket()
        {
            if (HttpContext.Session.GetString("UserRole") != "Distributor")
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddBlanket(Blanket blanket)
        {
            if (HttpContext.Session.GetString("UserRole") != "Distributor")
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                return View(blanket);
            }

            var client = _clientFactory.CreateClient("CozyComfortAPI");
            var json = JsonSerializer.Serialize(blanket);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("blankets", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Blanket added successfully! Stock has been updated.";
                return RedirectToAction(nameof(Index)); // Redirect back to dashboard
            }

            TempData["Error"] = "Failed to add blanket.";
            return View(blanket);
        }
        // ... existing code ...

        [HttpGet]
        public async Task<IActionResult> UpdateStock()
        {
            if (HttpContext.Session.GetString("UserRole") != "Distributor")
            {
                return RedirectToAction("Login", "Account");
            }

            // Fetch all blankets to populate the dropdown
            var client = _clientFactory.CreateClient("CozyComfortAPI");
            var response = await client.GetAsync("blankets");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var blankets = JsonSerializer.Deserialize<List<Blanket>>(data, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                // Pass the list to the view using ViewBag
                ViewBag.Blankets = blankets;
                return View();
            }

            return View(new List<Blanket>());
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStock(int blanketId, int additionalStock)
        {
            if (HttpContext.Session.GetString("UserRole") != "Distributor")
            {
                return RedirectToAction("Login", "Account");
            }

            if (additionalStock <= 0)
            {
                TempData["Error"] = "Stock amount must be greater than zero.";
                return RedirectToAction(nameof(UpdateStock));
            }

            var client = _clientFactory.CreateClient("CozyComfortAPI");

            // First, get the current blanket details
            var getResponse = await client.GetAsync($"blankets/{blanketId}");
            if (!getResponse.IsSuccessStatusCode)
            {
                TempData["Error"] = "Blanket not found.";
                return RedirectToAction(nameof(UpdateStock));
            }

            var blanketData = await getResponse.Content.ReadAsStringAsync();
            var blanket = JsonSerializer.Deserialize<Blanket>(blanketData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // Calculate new stock level
            blanket.StockLevel += additionalStock;

            // Send PUT request to update the blanket
            var json = JsonSerializer.Serialize(blanket);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var putResponse = await client.PutAsync($"blankets/{blanketId}", content);

            if (putResponse.IsSuccessStatusCode)
            {
                TempData["Success"] = $"Successfully added {additionalStock} units to '{blanket.ModelName}'. New Stock: {blanket.StockLevel}";
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = "Failed to update stock.";
            return RedirectToAction(nameof(UpdateStock));
        }
    }
}