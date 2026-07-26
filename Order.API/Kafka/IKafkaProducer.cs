namespace Order.API.Kafka
{
    public interface IKafkaProducer
    {
        Task PublishAsync<T>(
            string topic,
            string key,
            T message);
    }
}
