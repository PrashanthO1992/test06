namespace Rediscache.Models
{
    public class Items
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; } = "";
        public string CustomerName { get; set; } = "";
        public string ProductName { get; set; } = "";
        public decimal Amount { get; set; }
    }
}
