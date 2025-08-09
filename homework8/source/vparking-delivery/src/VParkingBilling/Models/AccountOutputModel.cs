namespace VParkingBilling.Models;

/// <summary>
/// Модель клиента
/// </summary>
public record AccountOutputModel
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Идентификатор клиента
    /// </summary>
    public required string ClientId { get; init; }
    
}