using Microsoft.AspNetCore.Mvc;
using SmartGearApp.Models;
using System.Linq;

namespace SmartGearApp.ViewComponents
{
    public class ProductSummaryViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;

        public ProductSummaryViewComponent(AppDbContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            var totalProducts = _context.Products.Count();
            var avgPrice = _context.Products.Any() ? _context.Products.Average(p => p.Price) : 0;
            ViewData["Total"] = totalProducts;
            ViewData["AvgPrice"] = avgPrice;
            return View();
        }
    }
}
