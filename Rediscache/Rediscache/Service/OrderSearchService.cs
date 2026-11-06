using Elastic.Clients.Elasticsearch;
using StackExchange.Redis;

namespace Rediscache.Service
{
    public class OrderSearchService
    {
        private readonly ElasticsearchClient _client;

        public OrderSearchService(
            ElasticsearchClient client)
        {
            _client = client;
        }

        public async Task IndexOrderAsync(Items order)
        {
            var response =
                await _client.IndexAsync(
                    order,
                    x => x
                        .Index("orders")
                        .Id(order.OrderId));

            if (!response.IsValidResponse)
            {
                throw new Exception(
                    "Failed to index order");
            }
        }


        public async Task<List<Order>> SearchAsync(
            string keyword)
        {
            var response =
                await _client.SearchAsync<Order>(
                    s => s
                        .Indices("orders")
                        .Query(q => q
                            .MultiMatch(m => m
                                .Fields(
                                    new[]
                                    {
                                "orderNumber",
                                "customerName",
                                "productName"
                                    })
                                .Query(keyword)))
                        .Size(20));

            if (!response.IsValidResponse)
            {
                throw new Exception(
                    "Elasticsearch search failed");
            }

            return response.Documents.ToList();
        }
    }
}

