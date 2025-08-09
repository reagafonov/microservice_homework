using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Services.Contracts;

namespace Services.Abstractions;

/// <summary>
/// Сервис работы с клиентами (интерфейс)
/// </summary>
public interface IOrderService
{
    /// <summary>
    /// Получить список
    /// </summary>
    /// <param name="page">номер страницы</param>
    /// <param name="pageSize">объем страницы</param>
    /// <param name="filter"></param>
    /// <returns>Список клиентов</returns>
    Task<ICollection<OrderDto>> GetPaged(int page, int pageSize, OrderFilterDto filter);

    /// <summary>
    /// Получить
    /// </summary>
    /// <param name="id">идентификатор</param>
    /// <returns>ДТО клиента</returns>
    Task<OrderDto> GetById(Guid id);

    /// <summary>
    /// Создать
    /// </summary>
    /// <param name="orderDto">ДТО клиента</param>
    /// <returns>идентификатор</returns>
    Task<Guid> Create(OrderDto orderDto);

    /// <summary>
    /// Изменить
    /// </summary>
    /// <param name="id">идентификатор</param>
    /// <param name="orderDto">ДТО клиента</param>
    Task<OrderDto> Update(Guid id, OrderDto orderDto);

    /// <summary>
    /// Удалить
    /// </summary>
    /// <param name="id">идентификатор</param>
    Task Delete(Guid id);
}