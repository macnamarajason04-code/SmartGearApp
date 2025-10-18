using Microsoft.Extensions.Caching.Memory;
using SmartGearApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartGearApp.Services
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _cache;
        private const string ProductCacheKey = "product_list";

        public ProductRepository(AppDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            if (!_cache.TryGetValue(ProductCacheKey, out IEnumerable<Product>? products))
            {
                Console.WriteLine("[CACHE MISS] Fetching products from database...");
                products = _context.Products.ToList(); // synchronous for simplicity

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5));

                _cache.Set(ProductCacheKey, products, cacheOptions);
            }
            else
            {
                Console.WriteLine("[CACHE HIT] Loading products from memory cache...");
            }

            return products!;
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return _context.Products.FirstOrDefault(p => p.Id == id);
        }

        public async Task AddAsync(Product product)
        {
            _context.Products.Add(product);
            _cache.Remove(ProductCacheKey); // clear cache after new data
            Console.WriteLine("[CACHE CLEARED] Product added, cache refreshed.");
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            _cache.Remove(ProductCacheKey);
            Console.WriteLine("[CACHE CLEARED] Product updated, cache refreshed.");
        }

        public async Task DeleteAsync(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                _context.Products.Remove(product);
                _cache.Remove(ProductCacheKey);
                Console.WriteLine("[CACHE CLEARED] Product deleted, cache refreshed.");
            }
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
