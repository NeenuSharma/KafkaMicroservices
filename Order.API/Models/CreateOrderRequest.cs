namespace Order.API.Models
{
    public class CreateOrderRequest
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public decimal TotalAmount { get; set; }
    }
}
