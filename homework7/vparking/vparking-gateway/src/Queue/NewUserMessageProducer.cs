using AutoMapper;
using Common.Infrastructure.Queue;
using Common.Users;
using Consumer.Options;
using Services.Repositories.Abstractions;

namespace keycloak_userEditor.Queue;

public class NewUserMessageProducer(
    IMapper mapper,
    ILogger<BaseProducer<string, NewUserEventMessage>> logger,
    KafkaOptions kafkaOptions)
    : QueueMessageProducer<NewUserEventMessage, NewUserEventMessage, string>(mapper, logger, kafkaOptions),
        INewUserMessageProducer
{
    protected override string Topic => kafkaOptions.Topic;
    protected override Func<NewUserEventMessage, string> KeySelector => message => message.ClientID;
}