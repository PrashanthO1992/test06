using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rediscache.Repositories;
using Rediscache.Service;

namespace Rediscache.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ProductRepository _repository;
        private readonly OrderSearchService _searchService;

        private readonly OrderCacheService _cache;

        public OrderController(OrderCacheService cache, OrderSearchService searchService, ProductRepository repository)
        { 
        _searchService = searchService;
            _cache = cache;
            _repository = repository;
        }

        [HttpGet("GetOrder")]
        public async Task<IActionResult> GetOrder(int id)
        {
            // 1. Check cache
            var cached =
                await _cache.GetAsync(id);

            if (cached != null)
            {
                return Ok(new
                {
                    Source = "ElastiCache",
                    Data = cached
                });
            }

            // 2. Cache miss → SQL Server
            var order =
                await _repository.GetProductByIdAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            // 3. Save to cache
            await _cache.SetAsync(order);

            return Ok(new
            {
                Source = "SQL Server",
                Data = order
            });
        }
    }
}
