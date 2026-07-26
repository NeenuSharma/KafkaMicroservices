using Microsoft.AspNetCore.Mvc;
using Order.API.Kafka;
using Order.API.Models;
using Shared.Events.Events;

namespace Order.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IKafkaProducer _kafkaProducer;

        public OrderController(IKafkaProducer kafkaProducer)
        {
            _kafkaProducer = kafkaProducer;
        }
        [HttpPost]
        public async Task<IActionResult> CreateOrder(
        CreateOrderRequest request)
        {
            var orderCreatedEvent = new OrderCreatedEvent
            {
                OrderId = Random.Shared.Next(1000, 9999),
                ProductId = request.ProductId,
                ProductName = request.ProductName,
                Quantity = request.Quantity,
                TotalAmount = request.TotalAmount,
                CreatedOn = DateTime.UtcNow
            };

            await _kafkaProducer.PublishAsync(
                topic: "orders",
                key: $"Order-{orderCreatedEvent.OrderId}",
                message: orderCreatedEvent);

            return Ok(new
            {
                Message = "Order published successfully.",
                OrderId = orderCreatedEvent.OrderId
            });
        }
    }
}
