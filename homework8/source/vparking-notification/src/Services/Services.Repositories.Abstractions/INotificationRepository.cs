using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;

namespace Services.Repositories.Abstractions;

/// <summary>
/// Интерфейс репозитория работы со счетами
/// </summary>
public interface INotificationRepository : IRepository<Notification, Guid>
{
    /// <summary>
    /// Получить постраничный список
    /// </summary>
    /// <param name="page">номер страницы</param>
    /// <param name="itemsPerPage">объем страницы</param>
    /// <param name="filter"></param>
    /// <returns> Список счетов</returns>
    Task<List<Notification?>> GetPagedAsync(int page, int itemsPerPage, AccountFilter? filter);
}