using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Shared.Events.Events;
using System.Text.Json;

namespace Inventory.API.Consumers;

public class InventoryConsumer : BackgroundService
{
    private readonly IConfiguration _configuration;

    public InventoryConsumer(
        IConfiguration configuration)
    {
        _configuration = configuration;
    }
    protected override async Task ExecuteAsync(
     CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers =
                _configuration["Kafka:BootstrapServers"],

            GroupId =
                _configuration["Kafka:GroupId"],

            AutoOffsetReset =
                AutoOffsetReset.Earliest
        };

        using var consumer =
            new ConsumerBuilder<string, string>(config)
                .Build();

        consumer.Subscribe(
            _configuration["Kafka:Topic"]);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var consumeResult =
    consumer.Consume(stoppingToken);

                var order =
                    JsonSerializer.Deserialize<OrderCreatedEvent>(
                        consumeResult.Message.Value);
                Console.WriteLine("-----------------------------------");

                Console.WriteLine($"Order Id      : {order!.OrderId}");
                Console.WriteLine($"Product Id    : {order.ProductId}");
                Console.WriteLine($"Product Name  : {order.ProductName}");
                Console.WriteLine($"Quantity      : {order.Quantity}");
                Console.WriteLine($"Total Amount  : {order.TotalAmount}");
                Console.WriteLine($"Created On    : {order.CreatedOn}");

                Console.WriteLine("-----------------------------------");
            }
        }
        catch (OperationCanceledException)
        {
            consumer.Close();
        }

        await Task.CompletedTask;
    }
}