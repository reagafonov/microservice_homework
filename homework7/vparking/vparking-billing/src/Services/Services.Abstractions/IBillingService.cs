using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Services.Contracts;

namespace Services.Abstractions;

/// <summary>
/// Сервис работы с клиентами (интерфейс)
/// </summary>
public interface IBillingService 
{
    /// <summary>
    /// Получить список
    /// </summary>
    /// <param name="page">номер страницы</param>
    /// <param name="pageSize">объем страницы</param>
    /// <param name="filter"></param>
    /// <returns>Список клиентов</returns>
    Task<ICollection<BillingDto>> GetPaged(int page, int pageSize, AccountFilterDto filter);

    /// <summary>
    /// Получить
    /// </summary>
    /// <param name="id">идентификатор</param>
    /// <returns>ДТО клиента</returns>
    Task<BillingDto> GetById(Guid id);

    /// <summary>
    /// Создать
    /// </summary>
    /// <param name="billingDto">ДТО клиента</param>
    /// <returns>идентификатор</returns>
    Task<Guid> Create(BillingDto billingDto);

    /// <summary>
    /// Изменить
    /// </summary>
    /// <param name="id">идентификатор</param>
    /// <param name="billingDto">ДТО клиента</param>
    Task<BillingDto> Update(Guid id, BillingDto billingDto);

    /// <summary>
    /// Удалить
    /// </summary>
    /// <param name="id">идентификатор</param>
    Task Delete(Guid id);

    Task<bool> ChangeBalance(ChangeBallanceDto changeBallanceDto);
    Task<bool> Pay(PaymentDto dto);
    
    Task<bool> RegisterUser(NewUserDto userDto);
}