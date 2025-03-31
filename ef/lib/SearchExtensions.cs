using Microsoft.EntityFrameworkCore;

namespace Persic;

public static class SearchExtensions
{
    public static async Task<T?> Search<T, TKey>(this IQueryable<T> dbSet, TKey id) 
        where T : class, IDbEntity<TKey>
    {
        return await dbSet.FirstOrDefaultAsync(x => x.Id!.Equals(id));
    }
}