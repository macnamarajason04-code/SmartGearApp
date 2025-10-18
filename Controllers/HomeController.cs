using Microsoft.AspNetCore.Mvc;
using SmartGearApp.Services;
using System.Diagnostics;
using SmartGearApp.Models;

namespace SmartGearApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRequestLogger _logger;

        // Dependency injection of IRequestLogger
        public HomeController(IRequestLogger logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            // Log via the injected service
            await _logger.LogAsync(HttpContext.Request);
            ViewData["Message"] = "Welcome to SmartGear — Middleware & DI are working 🚀";
            return View();
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
