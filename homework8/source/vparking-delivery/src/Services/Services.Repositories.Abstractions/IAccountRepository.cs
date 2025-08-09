using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;

namespace Services.Repositories.Abstractions;

/// <summary>
/// Интерфейс репозитория работы со счетами
/// </summary>
public interface IAccountRepository : IRepository<Account, Guid>
{
    /// <summary>
    /// Получить постраничный список
    /// </summary>
    /// <param name="page">номер страницы</param>
    /// <param name="itemsPerPage">объем страницы</param>
    /// <param name="filter"></param>
    /// <returns> Список счетов</returns>
    Task<List<Account?>> GetPagedAsync(int page, int itemsPerPage, AccountFilter filter);
    
    Task<Account?> GetByClientId(string clientId);
}