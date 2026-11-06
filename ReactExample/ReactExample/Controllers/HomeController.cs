using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using ReactExample.Services;

namespace ReactExample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IHomeservice homeService;
        private readonly string Connectionstring;
        private readonly string Queuename;
        public HomeController(IHomeservice _homeservice, IConfiguration config)
        {
            homeService = _homeservice;
            Connectionstring = config["AzureAd:ConnectionString"]!;
            Queuename = config["AzureAd:QueueName"];
        }

        [HttpGet(nameof(GetOrders))]
        public async Task<IActionResult> GetOrders()
        {
            var result = await homeService.GetOrder();
            return Ok(result);
        }
        /// <summary>
        /// SENDER SERVICEBUS
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        [HttpPost(nameof(ServiceBusSender))]
        public async Task ServiceBusSender(Items items)
        {

            var client = new ServiceBusClient(Connectionstring);

            var sender = client.CreateSender(Queuename);

            var message = new ServiceBusMessage(
                                        """
                            {
                                "OrderId": "ORD1001",
                                "CustomerId": "C100",
                                "Amount": 5000
                            }
                            """);

            message.MessageId = "ORD1001";

            await sender.SendMessageAsync(message);

        }
    }
}
