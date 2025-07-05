namespace Services.Abstractions;

/// <summary>
/// DTO фильтра счетов
/// </summary>
public class AccountFilterDto
{
    
    /// <summary>
    /// Шаблон поиска по идентификатору клиента
    /// </summary>
    public string? ClientId { get; init; }

    /// <summary>
    /// Поиск по удаленным счетам
    /// </summary>
    public bool? IsDeleted { get; set; }
}