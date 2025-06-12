namespace Services.Contracts;

/// <summary>
/// Контракт оплаты
/// </summary>
public class PaymentModel
{
    /// <summary>
    /// Идентификатор клиента
    /// </summary>
    public string ClientID { get; set; }

    /// <summary>
    /// Цена
    /// </summary>
    public decimal Price { get; set; }
}