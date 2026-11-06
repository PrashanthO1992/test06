using Azure.Messaging.ServiceBus;
using Dapper;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Amqp.Framing;
using Rediscache.Models;
using Rediscache.Service;
using StackExchange.Redis;
using System.Text.Json;

namespace Rediscache.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private Dappercontext _dapper;
        private readonly IConfiguration _config;
        private readonly string Connectionstring;
        private readonly string Queuename;
        private readonly int aaa;
        private readonly string keyvaultkey;
 
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,

        };
        public HomeController(Dappercontext dapper, IConfiguration config)
        {
            _dapper = dapper;
            Connectionstring = config["AzureAd:ConnectionString"]!;
            Queuename = config["AzureAd:QueueName"];
            keyvaultkey = config["KeyVault:Url"];
         

        }
        [HttpGet(nameof(GetOrders))]
        public async Task<IActionResult> GetOrders()
        {
            int b = aaa;
            var orders = new List<Items>
        {
            new Items
            {
                OrderId = 1,
                OrderNumber = "ORD001",
                CustomerName = "John",
                ProductName = "Laptop",
                Amount = 75000
            },
            new Items
            {
                OrderId = 2,
                OrderNumber = "ORD002",
                CustomerName = "David",
                ProductName = "Mobile",
                Amount = 45000
            }
        };

            return Ok(orders);
        }

        [HttpGet(nameof(GetOrder))]
        public async Task<IActionResult> GetOrder()
        {
            return Ok(1);
        }

        [HttpGet(nameof(GetItems))]
        public async Task<IActionResult> GetItems()
        {
            using var connection = _dapper.createconnection();
            var query = "select * from dbo.Items";
            var result = await connection.QueryAsync<Items>(query);
            return Ok(result);
        }
        /// <summary>
        /// ServiceReceiver
        /// </summary>
        /// <returns></returns>
        [HttpPost(nameof(ServiceReceiver))]
        public async Task<Items> ServiceReceiver()
        {
            await using var client = new ServiceBusClient(Connectionstring);
            await using var receiver = client.CreateReceiver(Queuename);

            // Receive maximum 1 message and wait up to 5 seconds
            var messages = await receiver.ReceiveMessagesAsync(maxMessages: 1, maxWaitTime: TimeSpan.FromSeconds(5));

            // No message received
            if (messages.Count == 0)
            {
                return null;
            }

            // Get the first message
            var receivedMessage = messages[0];

            try
            {
                var body = receivedMessage.Body.ToString();
                var order = JsonSerializer.Deserialize<Items>(body, _jsonOptions);

                if (order != null)
                {
                    // Successfully processed
                    await receiver.CompleteMessageAsync(receivedMessage);
                    return order;
                }

                // JSON deserialization failed
                await receiver.DeadLetterMessageAsync(receivedMessage, "DeserializationFailed", "Order JSON could not be deserialized");
                return null;
            }
            catch (Exception ex)
            {
                await receiver.DeadLetterMessageAsync(receivedMessage, "ProcessingFailed", $"Exception during processing: {ex.Message}");

                throw;
            }
        }
        /// <summary>
        /// key vault
        /// </summary>
        /// <returns></returns>
        [HttpGet("key")]
        public IActionResult GetPaymentKey()
        {
            var paymentApiKey = keyvaultkey;

            return Ok(new
            {
                Message = "Secret was loaded successfully"
            });
        }

       
    }
}
