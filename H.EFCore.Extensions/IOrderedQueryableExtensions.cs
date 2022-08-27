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
    /// <inheritdoc cref="Queryable.OrderBy{TSource, TKey}(IQueryable{TSource}, Expression{Func{TSource, TKey}})"/>
    public static IOrderedQueryable<T> OrderThenBy<T>(this IQueryable<T> source, string propertyName)
    {
        return source.Order(source.IsOrder() ? nameof(Queryable.ThenBy) : nameof(Queryable.OrderBy), propertyName);
    }

    /// <summary>
    /// Automatically determine whether to use <see langword="OrderByDescending"/> or <see langword="ThenByDescending"/> according to whether it has been sorted
    /// </summary>
    /// <inheritdoc cref="Queryable.OrderBy{TSource, TKey}(IQueryable{TSource}, Expression{Func{TSource, TKey}})"/>
    public static IOrderedQueryable<T> OrderThenByDescending<T>(this IQueryable<T> source, string propertyName)
    {
        return source.Order(source.IsOrder() ? nameof(Queryable.ThenByDescending) : nameof(Queryable.OrderByDescending), propertyName);
    }

    /// <inheritdoc cref="Queryable.OrderByDescending{TSource, TKey}(IQueryable{TSource}, Expression{Func{TSource, TKey}})"/>
    public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string propertyName)
    {
        return source.Order(nameof(Queryable.OrderByDescending), propertyName);
    }

    /// <inheritdoc cref="Queryable.ThenBy{TSource, TKey}(IOrderedQueryable{TSource}, Expression{Func{TSource, TKey}})"/>
    public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> source, string propertyName)
    {
        return source.Order(nameof(Queryable.ThenBy), propertyName);
    }

    /// <inheritdoc cref="Queryable.ThenByDescending{TSource, TKey}(IOrderedQueryable{TSource}, Expression{Func{TSource, TKey}})"/>
    public static IOrderedQueryable<T> ThenByDescending<T>(this IOrderedQueryable<T> source, string propertyName)
    {
        return source.Order(nameof(Queryable.ThenByDescending), propertyName);
    }

    #endregion

    /// <remarks>
    /// <paramref name="orderString"/> Syntax: <c>column_name1 ASC|DESC, column_name2 ASC|DESC...</c>
    /// </remarks>
    /// <inheritdoc cref="Queryable.OrderBy{TSource, TKey}(IQueryable{TSource}, Expression{Func{TSource, TKey}})"/>
    public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string orderString)
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
        return ret as IOrderedQueryable<T> ?? throw new InvalidOperationException();
    }

    internal static IOrderedQueryable<T> Order<T>(this IQueryable<T> source, string orderMethodName, string propertyName)
    {
        var lambda = GetPropertyLambda<T>(propertyName);
        var method = typeof(Queryable).GetMethods()
            .Where(m => m.IsPublic && m.IsStatic && m.IsGenericMethod)
            .Where(m => m.Name == orderMethodName && m.GetParameters().Length == 2)
            .SingleOrDefault()
            ?.MakeGenericMethod(typeof(T), lambda.ReturnType);

        _ = method ?? throw new ArgumentException("Method Not Found.", nameof(orderMethodName));
        var ret = method.Invoke(null, new object?[] { source, lambda });
        return ret as IOrderedQueryable<T> ?? throw new InvalidOperationException();
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
