namespace VParkingNotification.Models;

/// <summary>
/// Модель фильтра счетов
/// </summary>
/// <remarks>Решил не переносить сюда пагинацию</remarks>
public class AccountFilterModel
{
    /// <summary>
    /// Шаблон фильтра по идентификатору пользователя
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public string? ClientId { get; init; }

    /// <summary>
    /// Шаблон фильтра по признаку удаления
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public bool? IsDeleted { get; set; }
    
}