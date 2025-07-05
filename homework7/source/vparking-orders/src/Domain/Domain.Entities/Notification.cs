using System;

namespace Domain.Entities;

public class Notification
{
    public NotificationType MessageType { get; set; }

    public string ClientID { get; set; }
    
    public string Email { get; set; }

    public Guid OrderId { get; set; }
}