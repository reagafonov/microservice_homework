using System.ComponentModel.DataAnnotations;

namespace VParkingBilling.Models;

/// <summary>
/// Модель счета
/// </summary>
public record AccountInputModel
{

    /// <summary>
    /// Сумма на счете
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public decimal Amount { get; set; }
   
}