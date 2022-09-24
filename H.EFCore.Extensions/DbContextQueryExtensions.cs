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
    /// <param name="entity">Entity Instance</param>
    /// <returns>A set for <paramref name="entity"/>.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static IQueryable<TEntity> Query<TEntity>(this DbContext context, TEntity entity)
        where TEntity : class
    {
        return context.QueryObject<TEntity>(entity);
    }

    /// <remarks>Make sure <paramref name="entity"/> has the properties with same name and assignable type as keys of <typeparamref name="TEntity"/></remarks>
    /// <inheritdoc cref="Query{TEntity}(DbContext, TEntity)"/>
    public static IQueryable<TEntity> QueryObject<TEntity>(this DbContext context, object entity)
        where TEntity : class
    {
        return context.Set<TEntity>().Where(context.GetPredicate<TEntity>(new object[] { entity }));
    }

    /// <inheritdoc cref="QueryMultiple{TEntity}(DbContext, IEnumerable{TEntity})"/>
    public static IQueryable<TEntity> Query<TEntity>(this DbContext context, IEnumerable<TEntity> entities)
        where TEntity : class
    {
        return context.QueryMultiple(entities);
    }

    /// <summary>
    /// Query Entity Instance collection Set to <see cref="IQueryable{TEntity}"/>
    /// </summary>
    /// <typeparam name="TEntity">Entity Type</typeparam>
    /// <param name="context">The context instance that Includ <see cref="DbSet{TEntity}"/></param>
    /// <param name="entities">Entity Instance collection</param>
    /// <returns>A set for <paramref name="entities"/>.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static IQueryable<TEntity> QueryMultiple<TEntity>(this DbContext context, IEnumerable<TEntity> entities)
        where TEntity : class
    {
        return context.QueryObjects<TEntity>(entities);
    }

    /// <remarks>Make sure all of <paramref name="entities"/> have the properties with same name and assignable type as keys of <typeparamref name="TEntity"/></remarks>
    /// <inheritdoc cref="QueryMultiple{TEntity}(DbContext, IEnumerable{TEntity})"/>
    public static IQueryable<TEntity> QueryObjects<TEntity>(this DbContext context, IEnumerable<object> entities)
       where TEntity : class
    {
        return context.Set<TEntity>().Where(context.GetPredicate<TEntity>(entities));
    }

    internal static Expression<Func<TEntity, bool>> GetPredicate<TEntity>(this DbContext context, IEnumerable<object> entities)
        where TEntity : class
    {
        if (!entities.Any())
        {
            return e => false;
        }
        var eType = context.Model.FindEntityType(typeof(TEntity))
            ?? throw new InvalidOperationException(CoreStrings.InvalidSetType(nameof(TEntity)));
        var cache = context.GetService<IMemoryCache>();
        var keys = eType.GetUniquePropertyInfo(cache);

        if (entities.Count() == 1)
        {
            var parameter = Expression.Parameter(typeof(TEntity));
            var body = parameter.GetEqualCondition(entities.First(), keys);
            var lambda = Expression.Lambda<Func<TEntity, bool>>(body!, parameter);
            return lambda;
        }
        else if (keys.Count == 1 && entities is IEnumerable<TEntity> sameTypeSet)
        {
            return QueryMultipleWithSigleKey(sameTypeSet, keys[0]);
        }
        else
        {
            return QueryMultipleWithKeys<TEntity>(entities, keys);
        }
    }

    private static Expression<Func<TEntity, bool>> QueryMultipleWithSigleKey<TEntity>(IEnumerable<TEntity> entities, PropertyInfo key)
    {
        // t => entities.Select(e => e.[key]).Contains(t.[key])
        var parameter_t = Expression.Parameter(typeof(TEntity));
        var entitiesConst = Expression.Constant(entities);
        var parameter_e = Expression.Parameter(typeof(TEntity));
        var entitiesSelect = Expression.Call(
             typeof(Enumerable),
             nameof(Enumerable.Select),
             new[] { typeof(TEntity), key.PropertyType },
             entitiesConst,
             Expression.Lambda(Expression.Property(parameter_e, key), parameter_e));
        var entitiesContains = Expression.Call(
             typeof(Enumerable),
             nameof(Enumerable.Contains),
             new[] { key.PropertyType },
             entitiesSelect,
             Expression.Property(parameter_t, key));
        return Expression.Lambda<Func<TEntity, bool>>(entitiesContains, parameter_t);
    }

    private static Expression<Func<TEntity, bool>> QueryMultipleWithKeys<TEntity>(IEnumerable<object> entities, IEnumerable<PropertyInfo> keys)
    {
        // t => (t.[key1] == [key1] && t.[key2] == [key2]) || (t.[key1] == [key1] && t.[key2] == [key2])
        var parameter = Expression.Parameter(typeof(TEntity));
        var condutions = entities
            .Select(o => parameter.GetEqualCondition(o, keys))
            .OfType<Expression>();
        var body = condutions.OrElse();
        return Expression.Lambda<Func<TEntity, bool>>(body!, parameter);
    }

    /// <summary>
    ///     <para>
    ///     Begins tracking the given entities and entries reachable from the given entities using the <see cref="EntityState.Modified" /> state if the entity exists in the database, otherwise using the <see cref="EntityState.Added" /> state.
    ///     </para>
    ///     <para>
    ///         This method will query the database to determine whether to Added or Modified, but no data manipulation will be performed until <see cref="DbContext.SaveChanges()" /> is called.
    ///     </para>
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="context"></param>
    /// <param name="entities"></param>
    public static void Replace<TEntity>(this DbContext context, IEnumerable<TEntity> entities)
        where TEntity : class
    {
        var existData = context.Query(entities).AsNoTracking().ToList();
        var predicate = context.GetPredicate<TEntity>(existData);
        var loockup = entities.ToLookup(predicate.Compile());
        context.AddRange(loockup[false]);
        context.UpdateRange(loockup[true]);
    }
}
