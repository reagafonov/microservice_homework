using System.Linq;

namespace Services.Repositories.Abstractions;

/// <summary>
/// Фильтрация по полям класса из фильтра
/// </summary>
public interface ISimpleFilterQuery<TEntity, in TFilter>
{
    /// <summary>
    /// Добавляет запрос с фильтром по простым полям класса
    /// </summary>
    /// <param name="query">Исходный запрос</param>
    /// <param name="filter">Фильтр</param>
    /// <returns>Запрос с фильтр</returns>
    IQueryable<TEntity> Filter(IQueryable<TEntity> query, TFilter filter);
}