using System;

namespace Domain.Entities;

public class Order:IEntity<Guid>
{
    public Guid Id { get; set; }

    public string Data { get; set; }

    public string ClientID { get; set; }

    public bool IsVerified { get; set; }

    public bool IsPayed { get; set; }
    
    public string Email { get; set; }
    
    public bool IsDeleted { get; set; }
    
    public decimal Amount { get; set; }
}