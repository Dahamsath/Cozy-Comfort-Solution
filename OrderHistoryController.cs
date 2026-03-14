using CozyComfort.Client.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json;

namespace CozyComfort.Client.Controllers
{
    public class OrderHistoryController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;

        public OrderHistoryController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        
        public async Task<IActionResult> Index()
        {
            
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
            {
                return RedirectToAction("Login", "Account");
            }

            var client = _clientFactory.CreateClient("CozyComfortAPI");
            var response = await client.GetAsync("orders");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var orders = JsonSerializer.Deserialize<List<Order>>(data, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(orders);
            }
            return View(new List<Order>());
        }

        
        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
            {
                return RedirectToAction("Login", "Account");
            }

            var client = _clientFactory.CreateClient("CozyComfortAPI");
            await client.PutAsync($"orders/{id}/approve", null);
            return RedirectToAction(nameof(Index));
        }

        // PROTECTED: Checks login before allowing shipping
        [HttpPost]
        public async Task<IActionResult> Ship(int id)
        {
            // Step 7: Protect Your Pages
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
            {
                return RedirectToAction("Login", "Account");
            }

            var client = _clientFactory.CreateClient("CozyComfortAPI");
            await client.PutAsync($"orders/{id}/ship", null);
            return RedirectToAction(nameof(Index));
        }
    }
}