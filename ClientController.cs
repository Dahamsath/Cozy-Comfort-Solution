using CozyComfort.Client.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;

namespace CozyComfort.Client.Controllers
{
    public class ClientController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;

        public ClientController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("UserRole") != "Client")
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
        public IActionResult PlaceOrder()
        {
            if (HttpContext.Session.GetString("UserRole") != "Client")
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder(Order order)
        {
            if (HttpContext.Session.GetString("UserRole") != "Client")
            {
                return RedirectToAction("Login", "Account");
            }

            // Automatically set the SellerName to the logged-in Client's username
            order.SellerName = HttpContext.Session.GetString("Username");

            var client = _clientFactory.CreateClient("CozyComfortAPI");
            var json = JsonSerializer.Serialize(order);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("orders", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Order placed successfully! We will process it shortly.";
                return RedirectToAction("Index");
            }

            TempData["Error"] = "Failed to place order.";
            return View();
        }

        public async Task<IActionResult> MyOrders()
        {
            if (HttpContext.Session.GetString("UserRole") != "Client")
            {
                return RedirectToAction("Login", "Account");
            }

            var client = _clientFactory.CreateClient("CozyComfortAPI");
            var response = await client.GetAsync("orders");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var allOrders = JsonSerializer.Deserialize<List<Order>>(data, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                // Filter orders only for this specific client
                var username = HttpContext.Session.GetString("Username");
                var myOrders = allOrders.Where(o => o.SellerName == username).ToList();

                return View(myOrders);
            }
            return View(new List<Order>());
        }
    }
}