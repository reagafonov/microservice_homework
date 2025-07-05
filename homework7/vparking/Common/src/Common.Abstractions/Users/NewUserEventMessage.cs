using Common.Abstractions;

namespace Common.Users;

public class NewUserEventMessage : IKafkaMessage
{
    public string ClientID { get; set; }
}