using System.Linq.Expressions;

namespace Campaign.Infrastructure.Utils.Linq;

public static class QueryableExtensions
{
    /// <summary>
    /// Chunk list into multiple parts
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="chunkSize"></param>
    /// <returns></returns>
    public static List<List<T>> ChunkBy<T>(this List<T> source, int chunkSize)
    {
        return source
            .Select((x, i) => new { Index = i, Value = x })
            .GroupBy(x => x.Index / chunkSize)
            .Select(x => x.Select(v => v.Value).ToList())
            .ToList();
    }

    /// <summary>
    /// Filters a <see cref="IQueryable"/> by given predicate if given condition is true.
    /// </summary>
    /// <param name="query">Queryable to apply filtering</param>
    /// <param name="condition">A boolean value</param>
    /// <param name="predicate">Predicate to filter the query</param>
    /// <returns>Filtered or not filtered query based on <paramref name="condition"/></returns>
    public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, Expression<Func<T, bool>> predicate)
    {
        return condition
            ? query.Where(predicate)
            : query;
    }
    public static IEnumerable<T> WhereIf<T>(this IEnumerable<T> query, bool condition, Func<T, bool> predicate)
    {
        return condition
            ? query.Where(predicate)
            : query;
    }

    /// <summary>
    /// Filters a <see cref="IQueryable"/> by given predicate if given condition is true OR elsePredict if given condition is false.
    /// </summary>
    /// <param name="query">Queryable to apply filtering</param>
    /// <param name="condition">A boolean value</param>
    /// <param name="predicate">Predicate to filter the query if condition is true</param>
    /// <param name="elsePredicate">Predicate to filter the query if condition is false</param>
    /// <returns>Apply filtered queries based on <paramref name="condition"/></returns>
    public static IQueryable<T> WhereIfElse<T>(this IQueryable<T> query, bool condition, Expression<Func<T, bool>> predicate, Expression<Func<T, bool>> elsePredicate)
    {
        return condition
            ? query.Where(predicate)
            : query.Where(elsePredicate);
    }
    public static IEnumerable<T> WhereIfElse<T>(this IEnumerable<T> query, bool condition, Func<T, bool> predicate, Func<T, bool> elsePredicate)
    {
        return condition
            ? query.Where(predicate)
            : query.Where(elsePredicate);
    }

    public static Expression<Func<T, bool>> True<T>() { return f => true; }

    public static Expression<Func<T, bool>> False<T>() { return f => false; }

    public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
    {
        var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
        return Expression.Lambda<Func<T, bool>>
            (Expression.OrElse(expr1.Body, invokedExpr), expr1.Parameters);
    }

    public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
    {
        var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
        return Expression.Lambda<Func<T, bool>>
            (Expression.AndAlso(expr1.Body, invokedExpr), expr1.Parameters);
    }
}