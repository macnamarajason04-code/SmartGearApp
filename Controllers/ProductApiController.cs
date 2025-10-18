using Microsoft.AspNetCore.Mvc;
using SmartGearApp.Models;
using SmartGearApp.Services;
using SmartGearApp.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SmartGearApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductApiController : ControllerBase
    {
        private readonly IProductRepository _repository;
        private readonly IHubContext<ProductHub> _hubContext;

        public ProductApiController(IProductRepository repository, IHubContext<ProductHub> hubContext)
        {
            _repository = repository;
            _hubContext = hubContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var products = await _repository.GetAllAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            await _repository.AddAsync(product);
            await _repository.SaveAsync();

            await _hubContext.Clients.All.SendAsync("ReceiveUpdate", "New product added");

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, Product product)
        {
            if (id != product.Id)
                return BadRequest();

            await _repository.UpdateAsync(product);
            await _repository.SaveAsync();

            await _hubContext.Clients.All.SendAsync("ReceiveUpdate", "Product updated");

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            await _repository.DeleteAsync(id);
            await _repository.SaveAsync();

            await _hubContext.Clients.All.SendAsync("ReceiveUpdate", "Product deleted");

            return NoContent();
        }
    }
}
