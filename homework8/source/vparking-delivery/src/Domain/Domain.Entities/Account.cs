using System;

namespace Domain.Entities;

public class Account:IEntity<Guid>
{
    public Guid Id { get; set; }

    public decimal Amount { get; set; }

    public string ClientID { get; set; }

    public bool IsDeleted { get; set; }
    
}