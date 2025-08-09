namespace VParkingSettings.Models;

/// <summary>
/// Модель фильтра карточек клиентов
/// </summary>
/// <remarks>Решил не переносить сюда пагинацию</remarks>
public class OrderFilterModel
{
    /// <summary>
    /// Шаблон фильтра по идентификатору пользователя
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public string? ClientId { get; init; }

    /// <summary>
    /// Шаблон фильтра по признаку оплаченного заказа
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public bool? IsPayed { get; init; }
    
    /// <summary>
    /// Шаблон фильтра по признаку верифицированного заказа
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public bool? IsVerified { get; init; }
}