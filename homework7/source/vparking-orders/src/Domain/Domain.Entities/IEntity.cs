namespace Domain.Entities;

/// <summary>
/// Интерфейс сущности с идентификатором
/// </summary>
/// <typeparam name="TId">Тип идентификатора</typeparam>
public interface IEntity<TId>
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    // ReSharper disable once UnusedMemberInSuper.Global
    TId Id { get; set; }
}