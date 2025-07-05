using Common.Abstractions;
using Confluent.Kafka;
using Consumer.Options;
using Microsoft.Extensions.Logging;

namespace Common.Infrastructure.Queue;

public class BaseProducer<TKey, TValue> 
    where TValue : IKafkaMessage
{
    private readonly IProducer<TKey, TValue> _producer;

    public BaseProducer(KafkaOptions kafkaOptions, ILogger<BaseProducer<TKey, TValue>> logger)
    {
        var producerConfig = new ProducerConfig()
        {
            BootstrapServers = kafkaOptions.BootstrapServers,
            AllowAutoCreateTopics = true
        };
        _producer = new ProducerBuilder<TKey, TValue>(producerConfig)
            .SetErrorHandler((_, error) => logger.LogError(error.Reason))
            .SetLogHandler((_, message) =>
            {
                logger.LogInformation(message.Message);
            })
            .SetKeySerializer((ISerializer<TKey>)Serializers.Utf8)
            .SetValueSerializer(new KafkaMessageSerializer<TValue>())
            .Build();
    }
    
    public Task ProduceAsync(string topic, Message<TKey,TValue> message, CancellationToken cancellationToken = default) => 
        _producer.ProduceAsync(topic, message, cancellationToken);
}