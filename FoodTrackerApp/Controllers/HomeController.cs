using System.Diagnostics;
using FoodTrackerApp.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Serilog; 

namespace FoodTrackerApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        // Purpose: Log unhandled exceptions and show a friendly error page 
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            // Capture the exception details from the context 
            var exceptionDetails = HttpContext.Features.Get<IExceptionHandlerFeature>();

            if (exceptionDetails != null)
            {
                // Requirement: Log caught exceptions with details using Serilog 
                Log.Error(exceptionDetails.Error, "An unhandled exception occurred at {Path}",
                    exceptionDetails.Path);
            }

            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}