using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Services.Repositories.Abstractions;
using Services.Repositories.Abstractions.Exceptions;

namespace Infrastructure.Repositories.Implementations;

/// <summary>
/// Репозиторий чтения и записи
/// </summary>
/// <typeparam name="T">Тип сущности</typeparam>
/// <typeparam name="TPrimaryKey">Основной ключ</typeparam>
public abstract class Repository<T, TPrimaryKey>(DbContext context) : ReadRepository<T, TPrimaryKey>(context),
    IRepository<T, TPrimaryKey>
    where T : class, IEntity<TPrimaryKey>
{
    /// <summary>
    /// Удалить сущность
    /// </summary>
    /// <param name="id">ID удалённой сущности</param>
    /// <returns>была ли сущность удалена</returns>
    public virtual bool Delete(TPrimaryKey id)
    {
        var obj = EntitySet.Find(id);
        if (obj == null)
        {
            return false;
        }

        EntitySet.Remove(obj);
        return true;
    }

    /// <summary>
    /// Удалить сущность
    /// </summary>
    /// <param name="id">ID удалённой сущности</param>
    /// <returns>была ли сущность удалена</returns>
    public virtual async Task<bool> DeleteAsync(TPrimaryKey id)
    {
        var obj = await EntitySet.FindAsync(id);
        if (obj == null)
        {
            return false;
        }

        EntitySet.Remove(obj);
        return true;
    }

    /// <summary>
    /// Удалить сущность
    /// </summary>
    /// <param name="entity">сущность для удаления</param>
    /// <returns>была ли сущность удалена</returns>
    public virtual bool Delete(T? entity)
    {
        if (entity == null)
        {
            return false;
        }

        Context.Entry(entity).State = EntityState.Deleted;
        return true;
    }

    /// <summary>
    /// Удалить сущности
    /// </summary>
    /// <param name="entities">Коллекция сущностей для удаления</param>
    /// <returns>была ли операция завершена успешно</returns>
    public virtual bool DeleteRange(ICollection<T>? entities)
    {
        if (entities == null || entities.Count == 0)
        {
            return false;
        }

        EntitySet.RemoveRange(entities);
        return true;
    }

    /// <summary>
    /// Для сущности проставить состояние - что она изменена
    /// </summary>
    /// <param name="entity">сущность для изменения</param>
    public virtual void Update(T entity)
    {
        Context.Entry(entity).State = EntityState.Modified;
    }

    /// <summary>
    /// Добавить в базу одну сущность
    /// </summary>
    /// <param name="entity">сущность для добавления</param>
    /// <returns>добавленная сущность</returns>
    public virtual T Add(T entity)
    {
        var objToReturn = Context.Set<T>().Add(entity);
        return objToReturn.Entity;
    }

    /// <summary>
    /// Добавить в базу одну сущность
    /// </summary>
    /// <param name="entity">сущность для добавления</param>
    /// <returns>добавленная сущность</returns>
    public virtual async Task<T> AddAsync(T entity)
    {
        return (await Context.Set<T>().AddAsync(entity)).Entity;
    }

    /// <summary>
    /// Добавить в базу массив сущностей
    /// </summary>
    /// <param name="entities">массив сущностей</param>
    public virtual void AddRange(IEnumerable<T> entities)
    {
        var enumerable = entities as IList<T> ?? entities.ToList();
        Context.Set<T>().AddRange(enumerable);
    }

    /// <summary>
    /// Добавить в базу массив сущностей
    /// </summary>
    /// <param name="entities">массив сущностей</param>
    public virtual async Task AddRangeAsync(ICollection<T>? entities)
    {
        if (entities == null || entities.Count == 0)
        {
            return;
        }

        await EntitySet.AddRangeAsync(entities);
    }

    /// <summary>
    /// Сохранить изменения
    /// </summary>
    public virtual string? SaveChanges()
    {
        try
        {
            Context.SaveChanges();
        }
        catch (Exception e)
        {
            return e.InnerException?.Message ?? e.Message;
        }

        return null;
    }

    /// <summary>
    /// Сохранить изменения
    /// </summary>
    public virtual async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await Context.SaveChangesAsync(cancellationToken);
        }
        catch (AggregateException e)
        {
            var errors = string.Join('\n', e.InnerExceptions.Select(x => x.InnerException?.Message ?? x.Message));
            throw new CrudUpdateException(errors);
        }
        catch (Exception e)
        {
            var errors = e.InnerException?.Message ?? e.Message;
            throw new CrudUpdateException(errors);
        }
    }
}