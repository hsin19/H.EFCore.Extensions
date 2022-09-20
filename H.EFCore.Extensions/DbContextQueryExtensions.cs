using System.Linq.Expressions;
using System.Reflection;
using H.EFCore.Extensions.Tools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Caching.Memory;

namespace H.EFCore.Extensions;

/// <summary>
/// DbContext Query Instance Extensions Methods
/// </summary>
public static class DbContextQueryExtensions
{
    /// <summary>
    /// Query Entity Instance to <see cref="IQueryable{T}"/>
    /// </summary>
    /// <typeparam name="TEntity">Entity Type</typeparam>
    /// <param name="context">The context instance that Includ <see cref="DbSet{TEntity}"/></param>
    /// <param name="instance">Entity Instance</param>
    /// <returns>A set for <paramref name="instance"/>.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static IQueryable<TEntity> Query<TEntity>(this DbContext context, TEntity instance)
        where TEntity : class
    {
        return context.QueryObject<TEntity>(instance);
    }

    /// <remarks>Make sure <paramref name="instance"/> has the properties with same name and assignable type as keys of <typeparamref name="TEntity"/></remarks>
    /// <inheritdoc cref="Query{TEntity}(DbContext, TEntity)"/>
    public static IQueryable<TEntity> QueryObject<TEntity>(this DbContext context, object instance)
        where TEntity : class
    {
        var eType = context.Model.FindEntityType(typeof(TEntity))
            ?? throw new InvalidOperationException(CoreStrings.InvalidSetType(nameof(TEntity)));
        var cache = context.GetService<IMemoryCache>();
        var keys = eType.GetUniquePropertyInfo(cache);
        var parameter = Expression.Parameter(typeof(TEntity));
        var body = parameter.GetEqualCondition(instance, keys);
        var lambda = Expression.Lambda<Func<TEntity, bool>>(body!, parameter);
        return context.Set<TEntity>().Where(lambda);
    }

    /// <inheritdoc cref="QueryMultiple{TEntity}(DbContext, IEnumerable{TEntity})"/>
    public static IQueryable<TEntity> Query<TEntity>(this DbContext context, IEnumerable<TEntity> instances)
        where TEntity : class
    {
        return context.QueryMultiple(instances);
    }

    /// <summary>
    /// Query Entity Instance collection Set to <see cref="IQueryable{TEntity}"/>
    /// </summary>
    /// <typeparam name="TEntity">Entity Type</typeparam>
    /// <param name="context">The context instance that Includ <see cref="DbSet{TEntity}"/></param>
    /// <param name="instances">Entity Instance collection</param>
    /// <returns>A set for <paramref name="instances"/>.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static IQueryable<TEntity> QueryMultiple<TEntity>(this DbContext context, IEnumerable<TEntity> instances)
        where TEntity : class
    {
        return context.QueryObjects<TEntity>(instances);
    }

    /// <remarks>Make sure all of <paramref name="instances"/> have the properties with same name and assignable type as keys of <typeparamref name="TEntity"/></remarks>
    /// <inheritdoc cref="QueryMultiple{TEntity}(DbContext, IEnumerable{TEntity})"/>
    public static IQueryable<TEntity> QueryObjects<TEntity>(this DbContext context, IEnumerable<object> instances)
       where TEntity : class
    {
        var eType = context.Model.FindEntityType(typeof(TEntity))
            ?? throw new InvalidOperationException(CoreStrings.InvalidSetType(nameof(TEntity)));

        if (!instances.Any())
        {
            return context.Set<TEntity>().Where(e => false);
        }
        var cache = context.GetService<IMemoryCache>();
        var keys = eType.GetUniquePropertyInfo(cache);

        if (keys.Count == 1 && instances is IEnumerable<TEntity> sameTypeSet)
        {
            var lamba = QueryMultipleWithSigleKey(sameTypeSet, keys[0]);
            return context.Set<TEntity>().Where(lamba);
        }
        else
        {
            var lamba = QueryMultipleWithKeys<TEntity>(instances, keys);
            return context.Set<TEntity>().Where(lamba);
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

    private static Expression<Func<T, bool>> QueryMultipleWithKeys<T>(IEnumerable<object> objs, IEnumerable<PropertyInfo> keys)
    {
        // t => (t.[key1] == [key1] && t.[key2] == [key2]) || (t.[key1] == [key1] && t.[key2] == [key2])
        var parameter = Expression.Parameter(typeof(T));
        var condutions = objs
            .Select(o => parameter.GetEqualCondition(o, keys))
            .OfType<Expression>()
            .ToList();
        var body = condutions.OrElse();
        return Expression.Lambda<Func<T, bool>>(body!, parameter);
    }
}
