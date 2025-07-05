using System;
using Common.Abstractions;

namespace Services.Contracts;

/// <summary>
/// ДТО счета
/// </summary>
public class NotificationDto
{
    public MessageTypeEnum MessageType { get; set; }

    public string ClientID { get; set; }
    
    public string Email { get; set; }
    
    public Guid OrderId { get; set; }
}