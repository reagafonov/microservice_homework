using System;

namespace Services.Contracts;

/// <summary>
/// ДТО счета
/// </summary>
public class BillingDto
{
    /// <summary>
    /// Идентификатор счета
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Сумма на счете
    /// </summary>
    public decimal? Amount { get; init; }

    /// <summary>
    /// Идентификатор клиента
    /// </summary>
    public string? ClientID { get; set; }

}