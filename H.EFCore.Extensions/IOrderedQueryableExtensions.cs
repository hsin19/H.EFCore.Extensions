using System.Linq.Expressions;
using H.EFCore.Extensions.Tools;

namespace H.EFCore.Extensions;

/// <summary>
/// Extension Method for <see cref="IOrderedQueryable"/>
/// </summary>
public static class IOrderedQueryableExtensionsg
{
    #region Overload

    /// <summary>
    /// Automatically determine whether to use <see langword="OrderBy"/> or <see langword="ThenBy"/> according to whether it has been sorted
    /// </summary>
    /// <param name="propertyName"><inheritdoc cref="Order"/></param>
    /// <inheritdoc cref="Queryable.OrderBy{TSource, TKey}(IQueryable{TSource}, Expression{Func{TSource, TKey}})"/>
    public static IOrderedQueryable<TSource> OrderThenBy<TSource>(this IQueryable<TSource> source, string propertyName)
    {
        return source.Order(source.IsOrder() ? nameof(Queryable.ThenBy) : nameof(Queryable.OrderBy), propertyName);
    }

    /// <summary>
    /// Automatically determine whether to use <see langword="OrderByDescending"/> or <see langword="ThenByDescending"/> according to whether it has been sorted
    /// </summary>
    /// <param name="propertyName"><inheritdoc cref="Order"/></param>
    /// <inheritdoc cref="Queryable.OrderBy{TSource, TKey}(IQueryable{TSource}, Expression{Func{TSource, TKey}})"/>
    public static IOrderedQueryable<TSource> OrderThenByDescending<TSource>(this IQueryable<TSource> source, string propertyName)
    {
        return source.Order(source.IsOrder() ? nameof(Queryable.ThenByDescending) : nameof(Queryable.OrderByDescending), propertyName);
    }

    /// <param name="propertyName"><inheritdoc cref="Order"/></param>
    /// <inheritdoc cref="Queryable.OrderByDescending{TSource, TKey}(IQueryable{TSource}, Expression{Func{TSource, TKey}})"/>
    public static IOrderedQueryable<TSource> OrderByDescending<TSource>(this IQueryable<TSource> source, string propertyName)
    {
        return source.Order(nameof(Queryable.OrderByDescending), propertyName);
    }

    /// <param name="propertyName"><inheritdoc cref="Order"/></param>
    /// <inheritdoc cref="Queryable.ThenBy{TSource, TKey}(IOrderedQueryable{TSource}, Expression{Func{TSource, TKey}})"/>
    public static IOrderedQueryable<TSource> ThenBy<TSource>(this IOrderedQueryable<TSource> source, string propertyName)
    {
        return source.Order(nameof(Queryable.ThenBy), propertyName);
    }

    /// <param name="propertyName"><inheritdoc cref="Order"/></param>
    /// <inheritdoc cref="Queryable.ThenByDescending{TSource, TKey}(IOrderedQueryable{TSource}, Expression{Func{TSource, TKey}})"/>
    public static IOrderedQueryable<TSource> ThenByDescending<TSource>(this IOrderedQueryable<TSource> source, string propertyName)
    {
        return source.Order(nameof(Queryable.ThenByDescending), propertyName);
    }

    #endregion

    /// <summary>
    /// Sort the elements of a sequence like sql syntax
    /// </summary>
    /// <param name="orderString">Similar to sql order, Syntax: <c>column_name1 ASC|DESC, column_name2 ASC|DESC...</c> </param>
    /// <remarks>If the sequence is already sorted it will become a subsort.</remarks>
    /// <inheritdoc cref="Queryable.OrderBy{TSource, TKey}(IQueryable{TSource}, Expression{Func{TSource, TKey}})"/>
    public static IOrderedQueryable<TSource> OrderBy<TSource>(this IQueryable<TSource> source, string orderString)
    {
        var properties = orderString
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var ret = source;
        foreach (var property in properties)
        {
            var col = property.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (col.Length > 2)
            {
                throw new ArgumentException("Syntax Error", nameof(orderString));
            }

            var name = col[0];
            var isDesc = col.Length == 2 && col[1].Equals("DESC", StringComparison.OrdinalIgnoreCase);
            ret = isDesc ? ret.OrderThenByDescending(name) : ret.OrderThenBy(name);
        }
        return ret as IOrderedQueryable<TSource> ?? throw new InvalidOperationException();
    }

    /// <summary>
    /// Base Order Method
    /// </summary>
    /// <typeparam name="TSource">The type of elements of <paramref name="source"/></typeparam>
    /// <param name="source"></param>
    /// <param name="orderMethodName"></param>
    /// <param name="propertyName">Property name for sorting, supports complex names.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    internal static IOrderedQueryable<TSource> Order<TSource>(this IQueryable<TSource> source, string orderMethodName, string propertyName)
    {
        var lambda = GetPropertyLambda<TSource>(propertyName);
        var method = typeof(Queryable).GetMethods()
            .Where(m => m.IsPublic && m.IsStatic && m.IsGenericMethod)
            .Where(m => m.Name == orderMethodName && m.GetParameters().Length == 2)
            .SingleOrDefault()
            ?.MakeGenericMethod(typeof(TSource), lambda.ReturnType);

        _ = method ?? throw new ArgumentException("Method Not Found.", nameof(orderMethodName));
        var ret = method.Invoke(null, new object?[] { source, lambda });
        return ret as IOrderedQueryable<TSource> ?? throw new InvalidOperationException();
    }

    internal static bool IsOrder(this IQueryable source)
    {
        return source.Expression.Type.IsGenericType
            && source.Expression.Type.GetGenericTypeDefinition().IsAssignableTo(typeof(IOrderedQueryable<>));
    }

    internal static LambdaExpression GetPropertyLambda<T>(string propertyName)
    {
        var parameter = Expression.Parameter(typeof(T));
        var property = parameter.GetComplexProperty(propertyName)
            ?? throw new ArgumentException("Property not found.", nameof(propertyName));
        return Expression.Lambda(property, parameter);
    }
}
