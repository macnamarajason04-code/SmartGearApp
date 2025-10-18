using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SmartGearApp.Models;
using SmartGearApp.Filters;
using SmartGearApp.Services;
using SmartGearApp.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SmartGearApp.Controllers
{
    [ServiceFilter(typeof(LogActionFilter))]
    [ServiceFilter(typeof(AuthorizeActionFilter))]
    [Route("products")]
    public class ProductController : Controller
    {
        private readonly IProductRepository _repository;
        private readonly ILogger<ProductController> _logger;
        private readonly IMemoryCache _cache;
        private readonly IHubContext<ProductHub> _hubContext;

        public ProductController(
            IProductRepository repository,
            ILogger<ProductController> logger,
            IMemoryCache cache,
            IHubContext<ProductHub> hubContext)
        {
            _repository = repository;
            _logger = logger;
            _cache = cache;
            _hubContext = hubContext;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            try
            {
                if (!_cache.TryGetValue("ProductList", out IEnumerable<Product>? products) || products == null)
                {
                    products = await _repository.GetAllAsync() ?? new List<Product>();

                    var cacheOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromMinutes(5));

                    _cache.Set("ProductList", products, cacheOptions);
                }

                return View(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching product list.");
                return View("Error");
            }
        }

        [HttpGet("details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var product = await _repository.GetByIdAsync(id);
                if (product == null) return NotFound();
                return View(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching details for product ID {id}.");
                return View("Error");
            }
        }

        [HttpGet("create")]
        public IActionResult Create() => View();

        [HttpPost("create")]
        public async Task<IActionResult> Create(Product product)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _repository.AddAsync(product);
                    await _repository.SaveAsync();

                    _cache.Remove("ProductList");
                    await _hubContext.Clients.All.SendAsync("ReceiveUpdate", "A new product was added.");

                    return RedirectToAction("Index");
                }
                return View(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating new product.");
                return View("Error");
            }
        }

        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var product = await _repository.GetByIdAsync(id);
                if (product == null) return NotFound();
                return View(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching product for edit with ID {id}.");
                return View("Error");
            }
        }

        [HttpPost("edit/{id}")]
        public async Task<IActionResult> Edit(Product product)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _repository.UpdateAsync(product);
                    await _repository.SaveAsync();

                    _cache.Remove("ProductList");
                    await _hubContext.Clients.All.SendAsync("ReceiveUpdate", "A product was updated.");

                    return RedirectToAction("Index");
                }
                return View(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating product ID {product.Id}.");
                return View("Error");
            }
        }

        [HttpGet("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var product = await _repository.GetByIdAsync(id);
                if (product == null) return NotFound();
                return View(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading delete confirmation for product ID {id}.");
                return View("Error");
            }
        }

        [HttpPost("delete/{id}")]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            try
            {
                await _repository.DeleteAsync(id);
                await _repository.SaveAsync();

                _cache.Remove("ProductList");
                await _hubContext.Clients.All.SendAsync("ReceiveUpdate", "A product was deleted.");

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting product ID {id}.");
                return View("Error");
            }
        }
    }
}
