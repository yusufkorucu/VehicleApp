using System.Text.Json;
using Confluent.Kafka;

namespace Payment.Api.Messaging;
public class KafkaEventBus : IEventBus, IDisposable
{
    private readonly IProducer<string, string> _producer;

    public KafkaEventBus(string bootstrapServers)
    {
        var config = new ProducerConfig
        {
            BootstrapServers = bootstrapServers
        };

        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task PublishAsync<T>(T @event, string topic, string key = null) where T : class
    {
        var message = JsonSerializer.Serialize(@event);
        var kafkaMessage = new Message<string, string>
        {
            Key = key,
            Value = message
        };

        try
        {
            var deliveryResult = await _producer.ProduceAsync(topic, kafkaMessage);
            Console.WriteLine(
                $"Message delivered to {deliveryResult.Topic} [{deliveryResult.Partition}] at offset {deliveryResult.Offset}");
        }
        catch (ProduceException<string, string> e)
        {
            Console.WriteLine($"Delivery failed: {e.Error.Reason}");
        }
    }

    public void Dispose()
    {
        _producer.Flush(TimeSpan.FromSeconds(10));
        _producer.Dispose();
    }
}