using AutoMapper;
using Common.Abstractions;
using Confluent.Kafka;
using Consumer.Options;
using Microsoft.Extensions.Logging;
using Services.Abstractions;

namespace Common.Infrastructure.Queue;

public abstract class QueueMessageProducer<TModel,TOutModel, TKey>(IMapper mapper,
    ILogger<BaseProducer<TKey,TOutModel>> logger,
    KafkaOptions kafkaOptions
    ) : IQueueAsyncMessage<TModel>
where TOutModel:IKafkaMessage
{
    private BaseProducer<TKey, TOutModel> producer = new(kafkaOptions, logger);
    protected abstract string Topic { get; }

    protected abstract Func<TOutModel, TKey> KeySelector { get; }
    
    public async Task<bool> ProduceAsync(TModel notification)
    {
        try
        {
            var notificationMessage = mapper.Map<TOutModel>(notification);
            var message = new Message<TKey, TOutModel>()
            {
                Key = KeySelector(notificationMessage),
                Value = notificationMessage,

            };
            await producer.ProduceAsync(Topic, message);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return false;
    }
}