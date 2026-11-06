using Microsoft.AspNetCore.Mvc;
using Rediscache.Models;
using Dapper;
using Microsoft.AspNetCore.JsonPatch.Internal;

namespace Rediscache.Repositories
{
    public class ProductRepository
    {
        private readonly Dappercontext _dapper;

        public ProductRepository(Dappercontext dapper)
        {
            _dapper = dapper;
        }
        
        private readonly List<Product> _products = new()
    {
        new Product
        {
            Id = 1,
            Name = "Laptop",
            Price = 75000
        },
        new Product
        {
            Id = 2,
            Name = "Mobile",
            Price = 30000
        }
    };

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            // Simulate database delay
            await Task.Delay(2000);

            return _products.FirstOrDefault(x => x.Id == id);
        }

        public  async Task<IEnumerable<Items>?> GetItems()
        {
            using var connection = _dapper.createconnection();
            var query = "select * from dbo.Parameters";
            var result = await connection.QueryAsync<Items>(query);
            return result;
        }
    }
}
