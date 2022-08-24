using H.EFCore.Extensions.Tools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace H.EFCore.Extensions;

/// <summary>
/// DbContext Query Instance Extensions Methods
/// </summary>
public static class DbContextQueryExtensions
{
    /// <summary>
    /// Query Entity Instance to <see cref="IQueryable{T}"/>
    /// </summary>
    /// <typeparam name="T">Entity Type</typeparam>
    /// <param name="context"></param>
    /// <param name="instance">Entity Instance</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static IQueryable<T> Query<T>(this DbContext context, T instance)
        where T : class
    {
        return context.QueryObject<T>(instance);
    }

    /// <remarks>Make sure <paramref name="instance"/> has the properties with same name and assignable type as keys of <typeparamref name="T"/></remarks>
    /// <inheritdoc cref="Query{T}(DbContext, T)"/>
    public static IQueryable<T> QueryObject<T>(this DbContext context, object instance)
        where T : class
    {
        Microsoft.EntityFrameworkCore.Metadata.IEntityType eType = context.Model.FindEntityType(typeof(T))
            ?? throw new InvalidOperationException(CoreStrings.InvalidSetType(nameof(T)));

        List<PropertyInfo> keys = eType.GetUniquePropertyInfo();

        ParameterExpression parameter = Expression.Parameter(typeof(T));

        Expression? body = parameter.GetEqualCondition(instance, keys);

        var lambda = Expression.Lambda<Func<T, bool>>(body!, parameter);
        return context.Set<T>().Where(lambda);
    }

    /// <inheritdoc cref="QueryMultiple{T}(DbContext, IEnumerable{T})"/>
    public static IQueryable<T> Query<T>(this DbContext context, IEnumerable<T> set)
        where T : class
    {
        return context.QueryMultiple(set);
    }

    /// <summary>
    /// Query Entity Set to <see cref="IQueryable{T}"/>
    /// </summary>
    /// <typeparam name="T">Entity Type</typeparam>
    /// <param name="context"></param>
    /// <param name="set">Entity Set</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static IQueryable<T> QueryMultiple<T>(this DbContext context, IEnumerable<T> set)
        where T : class
    {
        return context.QueryObjects<T>(set);
    }

    /// <remarks>Make sure all of <paramref name="set"/> have the properties with same name and assignable type as keys of <typeparamref name="T"/></remarks>
    /// <inheritdoc cref="QueryMultiple{T}(DbContext, IEnumerable{T})"/>
    public static IQueryable<T> QueryObjects<T>(this DbContext context, IEnumerable<object> set)
       where T : class
    {
        Microsoft.EntityFrameworkCore.Metadata.IEntityType eType = context.Model.FindEntityType(typeof(T))
            ?? throw new InvalidOperationException(CoreStrings.InvalidSetType(nameof(T)));

        if (!set.Any())
        {
            return context.Set<T>().Where(e => false);
        }

        List<PropertyInfo> keys = eType.GetUniquePropertyInfo();

        if (keys.Count == 1 && set is IEnumerable<T> sameTypeSet)
        {
            Expression<Func<T, bool>> lamba = QueryMultipleWithSigleKey(sameTypeSet, keys[0]);
            return context.Set<T>().Where(lamba);
        }
        else
        {
            Expression<Func<T, bool>> lamba = QueryMultipleWithKeys<T>(set, keys);
            return context.Set<T>().Where(lamba);
        }
    }

    private static Expression<Func<T, bool>> QueryMultipleWithSigleKey<T>(IEnumerable<T> objs, PropertyInfo key)
    {
        // t => entities.Select(e => e.[key]).Contains(t.[key])
        ParameterExpression parameter_t = Expression.Parameter(typeof(T));
        ConstantExpression entitiesConst = Expression.Constant(objs);
        ParameterExpression parameter_e = Expression.Parameter(typeof(T));
        MethodCallExpression entitiesSelect = Expression.Call(
             typeof(Enumerable),
             nameof(Enumerable.Select),
             new[] { typeof(T), key.PropertyType },
             entitiesConst,
             Expression.Lambda(Expression.Property(parameter_e, key), parameter_e));
        MethodCallExpression entitiesContains = Expression.Call(
             typeof(Enumerable),
             nameof(Enumerable.Contains),
             new[] { key.PropertyType },
             entitiesSelect,
             Expression.Property(parameter_t, key));
        return Expression.Lambda<Func<T, bool>>(entitiesContains, parameter_t);
    }

    private static Expression<Func<T, bool>> QueryMultipleWithKeys<T>(IEnumerable<object> objs, IEnumerable<PropertyInfo> keys)
    {
        // t => (t.[key1] == [key1] && t.[key2] == [key2]) || (t.[key1] == [key1] && t.[key2] == [key2])
        ParameterExpression parameter = Expression.Parameter(typeof(T));
        Expression? body = null;
        foreach (object obj in objs)
        {
            body = body.OrElse(parameter.GetEqualCondition(obj, keys));
        }
        return Expression.Lambda<Func<T, bool>>(body!, parameter);
    }
}