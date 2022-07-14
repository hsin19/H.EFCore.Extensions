using System.Linq.Expressions;

namespace IQuerableExtensions;

public static class IQueryableExtensions
{
    public static IQueryable<TResult> Select<TSource, TMiddle, TResult>(
        this IQueryable<TSource> source,
        Expression<Func<TMiddle, TResult>> selector,
        Expression<Func<TSource, TMiddle>> middle)
        => source.Select(selector.LambdaConcatenation<TSource, TResult>(middle));

    public static IQueryable<TResult> Select<TSource, TMiddle1, TMiddle2, TResult>(
        this IQueryable<TSource> source,
        Expression<Func<TMiddle1, TMiddle2, TResult>> selector,
        Expression<Func<TSource, TMiddle1>> middle1,
        Expression<Func<TSource, TMiddle2>> middle2)
        => source.Select(selector.LambdaConcatenation<TSource, TResult>(new LambdaExpression[] { middle1, middle2 }));

    public static IQueryable<TResult> Select<TSource, TMiddle1, TMiddle2, TMiddle3, TResult>(
        this IQueryable<TSource> source,
        Expression<Func<TMiddle1, TMiddle2, TMiddle3, TResult>> selector,
        Expression<Func<TSource, TMiddle1>> middle1,
        Expression<Func<TSource, TMiddle2>> middle2,
        Expression<Func<TSource, TMiddle3>> middle3)
        => source.Select(selector.LambdaConcatenation<TSource, TResult>(new LambdaExpression[] { middle1, middle2, middle3 }));

    public static IQueryable<TResult> Select<TSource, TMiddle1, TMiddle2, TMiddle3, TMiddle4, TResult>(
        this IQueryable<TSource> source,
        Expression<Func<TMiddle1, TMiddle2, TMiddle3, TMiddle4, TResult>> selector,
        Expression<Func<TSource, TMiddle1>> middle1,
        Expression<Func<TSource, TMiddle2>> middle2,
        Expression<Func<TSource, TMiddle3>> middle3,
        Expression<Func<TSource, TMiddle4>> middle4)
        => source.Select(selector.LambdaConcatenation<TSource, TResult>(new LambdaExpression[] { middle1, middle2, middle3, middle4 }));
}
