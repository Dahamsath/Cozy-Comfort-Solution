using CozyComfort.Client.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CozyComfort.Client.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _clientFactory = clientFactory;
        }

        public async Task<IActionResult> Index()
        {
            
            try
            {
                var client = _clientFactory.CreateClient("CozyComfortAPI");
                var response = await client.GetAsync("blankets");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var blankets = JsonSerializer.Deserialize<List<Blanket>>(data, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    ViewBag.FeaturedProducts = blankets.Take(3).ToList(); 
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching featured products");
                ViewBag.FeaturedProducts = new List<Blanket>();
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> QuickOrder(Order order)
        {
            if (!ModelState.IsValid)
            {
                
                TempData["Error"] = "Please fill all required fields correctly.";
                return RedirectToAction("Index");
            }

            try
            {
                var client = _clientFactory.CreateClient("CozyComfortAPI");
                var json = JsonSerializer.Serialize(order);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("orders", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Order placed successfully! We will contact you shortly.";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["Error"] = "Failed to place order. Please check stock availability.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while placing your order.";
                return RedirectToAction("Index");
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}