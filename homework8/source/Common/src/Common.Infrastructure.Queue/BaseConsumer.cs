using Common.Abstractions;
using Confluent.Kafka;
using Consumer.Options;
using Consumer.Utils;
using Microsoft.Extensions.Logging;

namespace Common.Infrastructure.Queue;

public sealed class BaseConsumer<TKey, TValue> where TValue : IKafkaMessage
{
    private IConsumer<TKey, TValue> Consumer { get; }

    public BaseConsumer(KafkaOptions kafkaOptions, ILogger logger, string groupId)
    {
        var consumerConfig = new ConsumerConfig
        {
            GroupId = groupId,
            BootstrapServers = kafkaOptions.BootstrapServers,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false,
            SaslUsername = "user1",
            SaslPassword = "",
            
        };
        ConsumerBuilder<TKey, TValue> consumerBuilder = new ConsumerBuilder<TKey, TValue>(consumerConfig);
        Consumer = consumerBuilder
            .SetErrorHandler((_, error) => logger.LogError(error.Reason))
            .SetLogHandler((_, message) =>
            {
                logger.LogInformation(message.Message);
            })
            .SetKeyDeserializer((IDeserializer<TKey>)Deserializers.Utf8)
            .SetValueDeserializer(new KafkaMessageDeserializer<TValue>())
            .Build();
    }

    public void Subscribe(params IEnumerable<string> topics)
    {
        Consumer.Subscribe(topics);
    }

    public void Unsubscribe()
    {
        Consumer.Unsubscribe();
    }

    public ConsumeResult<TKey, TValue> Consume(TimeSpan timeout)
    {
        return Consumer.Consume(timeout);
    }

    public void Commit()
    {
        Consumer.Commit();
    }
}