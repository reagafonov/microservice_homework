namespace VParkingSettings.Models;

/// <summary>
/// Модель клиента
/// </summary>
public record OrderOutputModel
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Данные заказа
    /// </summary>
    public required string? Data { get; init; }

    /// <summary>
    /// Идентификатор клиента
    /// </summary>
    public required string ClientId { get; init; }
    
    /// <summary>
    /// Верифицирован?
    /// </summary>
    public required bool IsVerified { get; init; }
    
    /// <summary>
    /// Оплачен?
    /// </summary>
    public required bool IsPayed { get; init; }
    
    /// <summary>
    /// Email
    /// </summary>
    public required string Email { get; init; }
}