using System;

namespace Domain.Entities;

public class Product : IEntity<Guid>
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public long Number { get; set; }

    public long Reserved { get; set; }

    public long Sold { get; set; }
}