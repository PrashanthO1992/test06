using Microsoft.AspNetCore.Mvc;
using Rediscache.Models;
using Rediscache.Repositories;
using Rediscache.Service;

namespace Rediscache.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ProductRepository _repository;
        private readonly RedisCacheService _cache;

        public ProductsController(
            ProductRepository repository,
            RedisCacheService cache)
        {
            _repository = repository;
            _cache = cache;
        }

        [HttpGet]
        public async Task<IActionResult> GetProduct(int id)
        {
            string cacheKey = $"product:{id}";

            // 1. Check Redis
            var cachedProduct =
                await _cache.GetAsync<Product>(cacheKey);

            if (cachedProduct != null)
            {
                return Ok(new
                {
                    Source = "Redis Cache",
                    Data = cachedProduct
                });
            }

            // 2. Cache miss → Database
            var product =
                await _repository.GetProductByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            // 3. Store in Redis
            await _cache.SetAsync(
                cacheKey,
                product,
                TimeSpan.FromMinutes(10));

            // 4. Return response
            return Ok(new
            {
                Source = "Database",
                Data = product
            });
           
        }
    }
}
