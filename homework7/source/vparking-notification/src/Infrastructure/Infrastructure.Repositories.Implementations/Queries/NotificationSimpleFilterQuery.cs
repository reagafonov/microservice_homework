using System.Linq;
using Domain.Entities;
using Services.Repositories.Abstractions;

namespace Infrastructure.Repositories.Implementations.Queries;

public class NotificationSimpleFilterQuery : ISimpleFilterQuery<Notification, AccountFilter>
{
    /// <summary>
    /// Добавляет запрос с фильтром по свойствам счета
    /// </summary>
    /// <param name="query">Исходный запрос</param>
    /// <param name="filter">Фильтр</param>
    /// <returns>Запрос с фильтром</returns>
    public IQueryable<Notification> Filter(IQueryable<Notification> query, AccountFilter filter)
    {
        if (!string.IsNullOrWhiteSpace(filter.ClientID))
            query = query.Where(order => order.ClientID == filter.ClientID);
        if (filter.IsDeleted.HasValue)
            query = query.Where(order => order.IsDeleted);
        return query;
    }
}