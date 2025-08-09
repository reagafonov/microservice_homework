using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;

namespace Services.Repositories.Abstractions;

public interface IProductRepository:IRepository<Product, Guid>
{
    
    Task<bool> UpdateStatistics(Product product);
}