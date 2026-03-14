using CozyComfort.Client.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace CozyComfort.Client.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;

        public AccountController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var client = _clientFactory.CreateClient("CozyComfortAPI");
            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("auth/login", content);

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                var authResult = JsonSerializer.Deserialize<LoginResponseDto>(responseData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                
                HttpContext.Session.SetString("Username", authResult.Email);
                HttpContext.Session.SetString("UserRole", authResult.Role);

               
                if (authResult.Role == "Distributor")
                {
                    return RedirectToAction("Index", "Distributor");
                }
                else
                {
                    return RedirectToAction("Index", "Seller");
                }
            }

            ViewBag.Error = "Invalid username or password";
            return View(model);
        }

        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var client = _clientFactory.CreateClient("CozyComfortAPI");
            var registerData = new { model.Email, model.Password, model.Role }; 
            var json = JsonSerializer.Serialize(registerData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("auth/register", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Registration successful! Please login.";
                return RedirectToAction("Login");
            }

            ViewBag.Error = "Registration failed.";
            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}