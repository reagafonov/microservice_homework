using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;

namespace Services.Repositories.Abstractions;

/// <summary>
/// Интерфейс репозитория работы с заказами
/// </summary>
public interface IOrderRepository : IRepository<Order, Guid>
{
    /// <summary>
    /// Получить постраничный список
    /// </summary>
    /// <param name="page">номер страницы</param>
    /// <param name="itemsPerPage">объем страницы</param>
    /// <param name="filter"></param>
    /// <returns> Список заказов</returns>
    Task<List<Order?>> GetPagedAsync(int page, int itemsPerPage, OrderFilter filter);
}