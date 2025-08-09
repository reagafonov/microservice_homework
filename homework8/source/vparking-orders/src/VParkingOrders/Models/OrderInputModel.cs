namespace VParkingSettings.Models;

/// <summary>
/// Модель клиента
/// </summary>
public record OrderInputModel
{
    /// <summary>
    /// Данные заказа
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public string Data { get; set; }

    /// <summary>
    /// Сумма заказа
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public decimal Amount { get; set; }
   
}