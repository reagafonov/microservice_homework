namespace Services.Abstractions;

/// <summary>
/// DTO фильтра карточек клиентов
/// </summary>
public class OrderFilterDto
{
    /// <summary>
    /// Шаблон поиска по признаку прохождения верификации
    /// </summary>
    public bool? IsVerified { get; init; }

    /// <summary>
    /// Шаблон поиска по признаку прохождения оплаты
    /// </summary>
    public bool? IsPayed { get; init; }
    
    /// <summary>
    /// Шаблон поиска по идентификатору клиента
    /// </summary>
    public string? ClientId { get; init; }
}