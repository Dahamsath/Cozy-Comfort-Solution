using CozyComfort.Client.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace CozyComfort.Client.Controllers
{
    public class SellerController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;

        public SellerController(IHttpClientFactory clientFactory)
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
            var response = await client.GetAsync("blankets");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var blankets = JsonSerializer.Deserialize<List<Blanket>>(data, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(blankets);
            }
            return View(new List<Blanket>());
        }

        
        [HttpGet]
        public IActionResult CreateOrder()
        {
            
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        
        [HttpPost]
        public async Task<IActionResult> CreateOrder(Order order)
        {
            // Step 7: Protect Your Pages
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
            {
                return RedirectToAction("Login", "Account");
            }

            var client = _clientFactory.CreateClient("CozyComfortAPI");
            var json = JsonSerializer.Serialize(order);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await client.PostAsync("orders", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Success");
            }
            return View();
        }

        // PROTECTED: Checks login before showing success message
        public IActionResult Success()
        {
            // Step 7: Protect Your Pages
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }
    }
}