using AutoMapper;
using Common.Billing;
using Confluent.Kafka;
using Consumer.Consumers;
using Consumer.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Services.Abstractions;
using Services.Contracts;

namespace Infrastructure.Queue;

public class BillingConsumer(IMapper mapper,IBillingService service,KafkaOptions options, ILogger<BillingConsumer> logger, KafkaOptions kafkaOptions) :
    ConsumerBackgroundService<string, NewUserEventMessage>(logger, kafkaOptions)
{
    protected override string TopicName => options?.Topic ?? string.Empty;
    protected override async Task HandleAsync(ConsumeResult<string, NewUserEventMessage> message, CancellationToken cancellationToken)
    {
        var newUserEventMessage = message?.Message?.Value;
        logger.LogInformation($"Получено сообщение регистрации пользователя {newUserEventMessage?.ClientID ?? "неизвестный"}");
        var msg = message.Message.Value;
        if (msg == null)
            return;
        var dto = mapper.Map<NewUserDto>(newUserEventMessage);
        await service.RegisterUser(dto);
    }
}