using System.Threading.Tasks;
using Domain.Entities;

namespace Services.Repositories.Abstractions;

/// <summary>
/// Интерфейс репозитория, предназначенного для чтения
/// </summary>
/// <typeparam name="T">Тип Entity для репозитория</typeparam>
/// <typeparam name="TPrimaryKey">тип первичного ключа</typeparam>
public interface IReadRepository<T, in TPrimaryKey> : IRepository where T : IEntity<TPrimaryKey>
{
    /// <summary>
    /// Получить сущность по ID
    /// </summary>
    /// <param name="id">ID сущности</param>
    /// <returns>сущность</returns>
    Task<T?> GetAsync(TPrimaryKey id);
}