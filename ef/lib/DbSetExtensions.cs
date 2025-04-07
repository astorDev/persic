using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Persic;

public static class DbSetExtensions
{
    public static async Task<T> Put<T>(this DbSet<T> dbSet, T entity, Action<T, T>? updateOverwrite = null) where T : class, IDbEntity<string>
    {
        var existing = await dbSet.AsNoTracking().FirstOrDefaultAsync(e => e.Id == entity.Id);

        if (existing == null) dbSet.Add(entity);
        else {
            updateOverwrite?.Invoke(existing, entity);
            dbSet.Update(entity);
        }

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