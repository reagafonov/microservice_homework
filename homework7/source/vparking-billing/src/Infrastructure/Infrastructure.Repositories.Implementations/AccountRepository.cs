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
/// Репозиторий работы со счетами
/// </summary>
public class AccountRepository(
    DatabaseContext context,
    ISimpleFilterQuery<Account, AccountFilter> orderSimpleFilterQuery)
    : Repository<Account, Guid>(context), IAccountRepository
{
    /// <summary>
    /// Получить постраничный список
    /// </summary>
    /// <param name="page">номер страницы</param>
    /// <param name="itemsPerPage">объем страницы</param>
    /// <param name="filter">Фильтр запросов</param>
    /// <returns> Список счетов</returns>
    public async Task<List<Account?>> GetPagedAsync(int page, int itemsPerPage, AccountFilter filter)
    {
            var query = GetAll();

            query = orderSimpleFilterQuery.Filter(query, filter);

            return (await query.OrderBy(order=>order.Id)
                .Skip((page - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .ToListAsync())!;
        }

    public async Task<Account?> GetByClientId(string clientId)
    {
        var query = await GetAll().Where(x=>x.ClientID == clientId).ToListAsync();
        if (query.Count > 1)
            return null;
        return query.FirstOrDefault();
    }
}