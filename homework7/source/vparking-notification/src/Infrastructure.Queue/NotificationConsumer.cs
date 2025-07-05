using AutoMapper;
using Confluent.Kafka;
using Consumer.Consumers;
using Consumer.Options;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using Services.Abstractions;
using Services.Contracts;

namespace Infrastructure.Queue;

public class NotificationConsumer(
    ILogger<NotificationConsumer> logger,
    KafkaOptions kafkaOptions,
    INotificationService notificationService,
    IMapper mapper)
    : ConsumerBackgroundService<string, NotificationMessage>(logger, kafkaOptions)
{
    //TopicName = "mytopic4";

    protected override string TopicName { get; } = kafkaOptions.Topic;

    protected override async Task HandleAsync(ConsumeResult<string, NotificationMessage> message, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Notification {message.Message.Key} received");
        await notificationService.SendNotification(mapper.Map<NotificationDto>(message.Message.Value));
    }
}