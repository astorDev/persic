﻿using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Persic.EF;

public static class DbSetExtensions
{
    public static async Task<T> Put<T>(this DbSet<T> dbSet, T entity) where T : class, IDbEntity<string>
    {
        var existing = await dbSet.AsNoTracking().FirstOrDefaultAsync(e => e.Id == entity.Id);

        if (existing == null) dbSet.Add(entity);
        else dbSet.Update(entity);

        return entity;
    }

    public static async Task<T> Put<T>(
        this DbSet<T> dbSet, 
        T entity, 
        Expression<Func<T, bool>> idSearch
    ) where T : class
    {
        var existing = await dbSet.AsNoTracking().FirstOrDefaultAsync(idSearch);

        if (existing == null) dbSet.Add(entity);
        else dbSet.Update(entity);

        return entity;
    }
}

public interface IDbEntity<TKey>
{
    TKey Id { get; set; }
}