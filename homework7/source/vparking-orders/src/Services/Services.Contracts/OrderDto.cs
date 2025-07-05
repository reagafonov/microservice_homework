using System;

namespace Services.Contracts;

/// <summary>
/// ДТО заказа
/// </summary>
public class OrderDto
{
    /// <summary>
    /// Идентификатор заказа
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Данные заказа
    /// </summary>
    public string? Data { get; init; }

    /// <summary>
    /// Сумма заказа
    /// </summary>
    public decimal? Amount { get; init; }

    /// <summary>
    /// Идентификатор клиента
    /// </summary>
    public string? ClientID { get; set; }

    /// <summary>
    /// Верифицирован?
    /// </summary>
    public bool IsVerified { get; set; }

    /// <summary>
    /// Оплачен?
    /// </summary>
    public bool IsPayed { get; set; }
    
    /// <summary>
    /// Email
    /// </summary>
    public string Email { get; set; }
}