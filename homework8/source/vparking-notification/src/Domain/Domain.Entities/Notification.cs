using System;

namespace Domain.Entities;

public class Notification:IEntity<Guid>
{
    public Guid Id { get; set; }

    public string Email { get; set; }

    public string ClientID { get; set; }

    public Guid OrderId { get; set; }
    
    public DomainMessageTypeEnum MessageType { get; set; }
    
    public bool IsDeleted { get; set; }
    
}