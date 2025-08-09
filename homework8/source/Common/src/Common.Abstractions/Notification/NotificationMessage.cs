using Common.Abstractions;

namespace Domain.Entities;

public class NotificationMessage:IKafkaMessage
{
    public MessageTypeEnum MessageType { get; set; }

    public string ClientID { get; set; }
    
    public string Email { get; set; }
    
    public Guid OrderId { get; set; }
}