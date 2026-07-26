using Confluent.Kafka;
using System.Text.Json;

namespace Order.API.Kafka;

public class KafkaProducer : IKafkaProducer
{
    private readonly IConfiguration _configuration;

    public KafkaProducer(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task PublishAsync<T>(
        string topic,
        string key,
        T message)
    {
        var config = new ProducerConfig
        {
            BootstrapServers =
                _configuration["Kafka:BootstrapServers"]
        };

        using var producer =
            new ProducerBuilder<string, string>(config)
                .Build();

        var json =
            JsonSerializer.Serialize(message);

        var kafkaMessage =
            new Message<string, string>
            {
                Key = key,
                Value = json
            };

        await producer.ProduceAsync(
            topic,
            kafkaMessage);
    }
}