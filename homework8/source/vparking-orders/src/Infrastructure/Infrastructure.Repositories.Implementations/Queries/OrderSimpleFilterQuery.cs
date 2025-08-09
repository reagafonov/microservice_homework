using System.Linq;
using Domain.Entities;
using Services.Repositories.Abstractions;

namespace Infrastructure.Repositories.Implementations.Queries;

public class OrderSimpleFilterQuery : ISimpleFilterQuery<Order, OrderFilter>
{
    /// <summary>
    /// Добавляет запрос с фильтром по свойствам заказа
    /// </summary>
    /// <param name="query">Исходный запрос</param>
    /// <param name="filter">Фильтр</param>
    /// <returns>Запрос с фильтр</returns>
    public IQueryable<Order> Filter(IQueryable<Order> query, OrderFilter filter)
    {
        if (!string.IsNullOrWhiteSpace(filter.ClientID))
            query = query.Where(order => order.ClientID == filter.ClientID);
        if (filter.IsVerified.HasValue)
            query = query.Where(order => order.IsVerified);
        if (filter.IsPayed.HasValue)
            query = query.Where(order => order.IsPayed);
        return query;
    }
}