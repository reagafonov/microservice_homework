using AutoMapper;
using Common.Infrastructure.Queue;
using Consumer.Options;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using Services.Repositories.Abstractions;

namespace Infrastructure.Queue.Implementation;

public class NotificationMessageProducer(IMapper mapper,
    ILogger<BaseProducer<string,NotificationMessage>> logger,
    KafkaOptions kafkaOptions
    ) : QueueMessageProducer<Notification, NotificationMessage, string>(mapper, logger, kafkaOptions)
        ,INotificationAsyncMessage
{
 

    protected override string Topic => kafkaOptions.Topic;
    protected override Func<NotificationMessage, string> KeySelector => message => message.MessageType.ToString();
    
}