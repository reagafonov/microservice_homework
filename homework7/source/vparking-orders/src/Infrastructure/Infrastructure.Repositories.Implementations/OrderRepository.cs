using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Services.Repositories.Abstractions;

namespace Infrastructure.Repositories.Implementations;

/// <summary>
/// Репозиторий работы с клиентами
/// </summary>
public class OrderRepository(
    DatabaseContext context,
    ISimpleFilterQuery<Order, OrderFilter> orderSimpleFilterQuery)
    : Repository<Order, Guid>(context), IOrderRepository
{
    /// <summary>
    /// Получить постраничный список
    /// </summary>
    /// <param name="page">номер страницы</param>
    /// <param name="itemsPerPage">объем страницы</param>
    /// <param name="filter">Фильтр запросов</param>
    /// <returns> Список клиентов</returns>
    public async Task<List<Order?>> GetPagedAsync(int page, int itemsPerPage, OrderFilter filter)
    {
            var query = GetAll();

            query = orderSimpleFilterQuery.Filter(query, filter);

            return (await query.OrderBy(order=>order.Id)
                .Skip((page - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .ToListAsync())!;
        }
}