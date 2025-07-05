using System.Threading;
using System.Threading.Tasks;
using Domain.Entities;

namespace Services.Repositories.Abstractions;

/// <summary>
/// Базовый интерфейс всех репозиториев
/// </summary>
public interface IRepository;

/// <summary>
/// Описания общих методов для всех репозиториев
/// </summary>
/// <typeparam name="T">Тип Entity для репозитория</typeparam>
/// <typeparam name="TPrimaryKey">тип первичного ключа</typeparam>
public interface IRepository<T, in TPrimaryKey> : IReadRepository<T, TPrimaryKey>
    where T : IEntity<TPrimaryKey>
{
    /// <summary>
    /// Удалить сущность
    /// </summary>
    /// <param name="id">ID удалённой сущности</param>
    /// <returns>была ли сущность удалена</returns>
    // ReSharper disable once UnusedMember.Global
    Task<bool> DeleteAsync(TPrimaryKey id);


    /// <summary>
    /// Для сущности проставить состояние - что она изменена
    /// </summary>
    /// <param name="entity">сущность для изменения</param>
    void Update(T entity);

    /// <summary>
    /// Добавить в базу одну сущность
    /// </summary>
    /// <param name="entity">сущность для добавления</param>
    /// <returns>добавленная сущность</returns>
    Task<T> AddAsync(T entity);

    /// <summary>
    /// Сохранить изменения
    /// </summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}