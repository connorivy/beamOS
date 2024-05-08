using System.Linq.Expressions;

namespace BeamOs.Infrastructure.Extensions;

public static class IQueryableExtensions
{
    public static IQueryable<T> WhereIf<T>(
        this IQueryable<T> queryable,
        Expression<Func<T, bool>> predicate,
        bool condition
    )
    {
        if (condition)
        {
            return queryable.Where(predicate);
        }
        return queryable;
    }
}
