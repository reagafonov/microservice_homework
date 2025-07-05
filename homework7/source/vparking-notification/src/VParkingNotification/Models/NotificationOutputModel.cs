namespace VParkingNotification.Models;

/// <summary>
/// Модель клиента
/// </summary>
public record NotificationOutputModel
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Идентификатор клиента
    /// </summary>
    public required string ClientId { get; init; }

    public Guid ClientID { get; set; }
    
}
