using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using H.EFCore.Extensions.Tools;

namespace H.EFCore.Extensions;

/// <summary>
/// Extension Method for <see cref="IQueryable"/>
/// </summary>
public static class IQueryableExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <inheritdoc cref="Queryable.Select{TSource, TResult}(IQueryable{TSource}, Expression{Func{TSource, int, TResult}})"/>
    public static IQueryable<TResult> SelectWithDefault<TSource, TResult>(this IQueryable<TSource> query, Expression<Func<TSource, TResult>>? selector = null)
    {
        var paramter = selector?.Parameters.Single()
            ?? Expression.Parameter(typeof(TSource));
        var changeBinding = (selector?.Body as MemberInitExpression)?.Bindings
            ?? Enumerable.Empty<MemberBinding>();

        var resultProperties = typeof(TSource).GetProperties().Where(p => p.CanWrite).ToList();
        var properties = typeof(TSource)
            .GetProperties()
            .Where(ps => resultProperties.Any(pr => ps.Name == pr.Name && ps.PropertyType.IsAssignableTo(pr.PropertyType)))
            .Where(p => p.CanRead && !changeBinding.Any(b => b.Member.Name == p.Name))
            .Select(p => Expression.Bind(p, Expression.Property(paramter, p)))
            .OfType<MemberBinding>()
            .Concat(changeBinding);
        var newBody = Expression.MemberInit(Expression.New(typeof(TResult)), properties);
        return query.Select(Expression.Lambda<Func<TSource, TResult>>(newBody, paramter));
    }
}
