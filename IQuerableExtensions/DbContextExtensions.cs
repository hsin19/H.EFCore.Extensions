using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace IQuerableExtensions;

public static class DbContextExtensions
{
    public static IQueryable<T> Query<T>(this DbContext context, T obj)
        where T : class
    {
        var eType = context.Model.FindEntityType(typeof(T))
            ?? throw new InvalidOperationException(CoreStrings.InvalidSetType(nameof(T)));

        var keys = eType.GetUniquePropertyInfo();

        var parameter = Expression.Parameter(typeof(T));

        var body = parameter.GetEquelCondition(obj, keys);

        var lambda = Expression.Lambda<Func<T, bool>>(body!, parameter);
        return context.Set<T>().Where(lambda);
    }

    public static IQueryable<T> Query<T>(this DbContext context, IEnumerable<T> obj)
        where T : class
    {
        return context.QueryMultiple(obj);
    }

    public static IQueryable<T> QueryMultiple<T>(this DbContext context, IEnumerable<T> objs)
        where T : class
    {
        var eType = context.Model.FindEntityType(typeof(T))
            ?? throw new InvalidOperationException(CoreStrings.InvalidSetType(nameof(T)));

        var keys = eType.GetUniquePropertyInfo();

        if (keys.Count == 1)
        {
            var lamba = QueryMultipleWithSigleKey(objs, keys[0]);
            return context.Set<T>().Where(lamba);
        }
        else
        {
            var lamba = QueryMultipleWithKeys(objs, keys);
            return context.Set<T>().Where(lamba);
        }
    }

    private static Expression<Func<T, bool>> QueryMultipleWithSigleKey<T>(IEnumerable<T> objs, PropertyInfo key)
    {
        // t => entities.Select(e => e.[key]).Contains(t.[key])
        var parameter_t = Expression.Parameter(typeof(T));
        var entitiesConst = Expression.Constant(objs);
        var parameter_e = Expression.Parameter(typeof(T));
        var entitiesSelect = Expression.Call(
             typeof(Enumerable),
             nameof(Enumerable.Select),
             new[] { typeof(T), key.PropertyType },
             entitiesConst,
             Expression.Lambda(Expression.Property(parameter_e, key), parameter_e));
        var entitiesContains = Expression.Call(
             typeof(Enumerable),
             nameof(Enumerable.Contains),
             new[] { key.PropertyType },
             entitiesSelect,
             Expression.Property(parameter_t, key));
        return Expression.Lambda<Func<T, bool>>(entitiesContains, parameter_t);
    }

    private static Expression<Func<T, bool>> QueryMultipleWithKeys<T>(IEnumerable<T> objs, IEnumerable<PropertyInfo> keys)
    {
        // t => (t.[key1] == [key1] && t.[key2] == [key2]) || (t.[key1] == [key1] && t.[key2] == [key2])
        var parameter = Expression.Parameter(typeof(T));
        Expression? body = null;
        foreach (var obj in objs)
        {
            body = body.OrElse(parameter.GetEquelCondition(obj, keys));
        }
        return Expression.Lambda<Func<T, bool>>(body!, parameter);
    }
}
