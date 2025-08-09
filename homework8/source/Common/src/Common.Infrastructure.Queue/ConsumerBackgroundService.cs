using Common.Abstractions;
using Common.Infrastructure.Queue;
using Confluent.Kafka;
using Consumer.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Consumer.Consumers;

public abstract class ConsumerBackgroundService<TKey, TValue>(
    ILogger logger,
    KafkaOptions kafkaOptions) : BackgroundService
    where TValue : IKafkaMessage
{
    private readonly BaseConsumer<TKey, TValue> _baseConsumer =
        new(kafkaOptions, logger, kafkaOptions.GroupID);
    protected abstract string TopicName { get; }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _baseConsumer.Subscribe(TopicName);
        logger.LogInformation($"Start consumer topic {TopicName}");
        while (!stoppingToken.IsCancellationRequested)
        {
            await Consume(stoppingToken);
        }
        
        _baseConsumer.Unsubscribe();
        logger.LogInformation($"Stop consumer topic {TopicName}");
    }

    private async Task Consume(CancellationToken cancellationToken)
    {
        ConsumeResult<TKey, TValue> message = null;
        try
        {
            message = _baseConsumer.Consume(TimeSpan.FromSeconds(10)); //можно подставлять cancellationToken, но тогда под капотом он подставит свое прерывание 5 сек и непредсказуемое поведение

            if (message is null)
            {
                await Task.Delay(100, cancellationToken);
                return;
            }
            
            await HandleAsync(message, cancellationToken);
            _baseConsumer.Commit();
        }
        catch (Exception e)
        {
            var key = message is not null ? message.Message.Key.ToString() : "No key";
            var value = message is not null ? message.Message.Value.ToString() : "No value";
            logger.LogError(e, $"Error process message with key {key}, value {value}");
        }
    }

    protected abstract Task HandleAsync(ConsumeResult<TKey, TValue> message, CancellationToken cancellationToken);

    public override void Dispose()
    {
        base.Dispose();
    }
}