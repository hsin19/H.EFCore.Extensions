using System.Linq.Expressions;
using H.EFCore.Extensions.Tools;

namespace H.EFCore.Extensions;

/// <summary>
/// Concatenate lambda expressions
/// </summary>
public static class LambdaConnectExtensions
{
    /// <summary>
    /// Concatenate lambda expressions <br/>
    /// (<typeparamref name="TSource"/>) =<paramref name="paramters"/>=> (, , , ...) =<paramref name="subLambda"/>=>  (<typeparamref name="TResult"/>)
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="subLambda">(<paramref name="paramters"/>) => (<typeparamref name="TResult"/>)</param>
    /// <param name="paramters">(<typeparamref name="TSource"/>) => (<paramref name="paramters"/>)</param>
    /// <returns>(<typeparamref name="TSource"/>) => (<typeparamref name="TResult"/>)</returns>
    /// <exception cref="ArgumentException"></exception>
    public static Expression<Func<TSource, TResult>> LambdaConcatenation<TSource, TResult>(this LambdaExpression subLambda, params LambdaExpression[] paramters)
    {
        if (subLambda.Parameters.Count != paramters.Length)
            throw new ArgumentException($"The number of {nameof(paramters)} must be {subLambda.Parameters.Count}", nameof(paramters));
        if (subLambda.ReturnType != typeof(TResult))
            throw new ArgumentException($"{nameof(subLambda)} Should return {nameof(TResult)}", nameof(subLambda));

        var sourceParamter = Expression.Parameter(typeof(TSource));
        List<Expression> newParamters = new();
        for (int i = 0; i < paramters.Length; i++)
        {
            var paramter = paramters[i];
            if (paramter.Parameters.Count != 1)
                throw new ArgumentException($"{nameof(subLambda)} can only be 1 {nameof(TSource)}", $"{nameof(paramters)}[{i}]");
            newParamters.Add(paramter.Body.ReplaceParameter(paramter.Parameters[0], sourceParamter));
        }

        var newBody = subLambda.Body.ReplaceParameters(subLambda.Parameters, newParamters);
        var lambda = Expression.Lambda<Func<TSource, TResult>>(newBody, sourceParamter);
        return lambda;
    }

    #region LambdaConnect IQueryable Select 

    /// <remarks>
    /// <paramref name="middle"/> will be the parameters of the <paramref name="selector"/> in order
    /// </remarks>
    /// <inheritdoc cref="Queryable.Select{TSource, TResult}(IQueryable{TSource}, Expression{Func{TSource, int, TResult}})"/>
    public static IQueryable<TResult> Select<TSource, TMiddle, TResult>(
        this IQueryable<TSource> source,
        Expression<Func<TMiddle, TResult>> selector,
        Expression<Func<TSource, TMiddle>> middle)
    {
        return source.Select(selector.LambdaConcatenation<TSource, TResult>(middle));
    }

    /// <inheritdoc cref="Select{TSource, TMiddle, TResult}(IQueryable{TSource}, Expression{Func{TMiddle, TResult}}, Expression{Func{TSource, TMiddle}})"/>
    public static IQueryable<TResult> Select<TSource, TMiddle1, TMiddle2, TResult>(
        this IQueryable<TSource> source,
        Expression<Func<TMiddle1, TMiddle2, TResult>> selector,
        Expression<Func<TSource, TMiddle1>> middle1,
        Expression<Func<TSource, TMiddle2>> middle2)
    {
        return source.Select(selector.LambdaConcatenation<TSource, TResult>(middle1, middle2));
    }

    /// <inheritdoc cref="Select{TSource, TMiddle, TResult}(IQueryable{TSource}, Expression{Func{TMiddle, TResult}}, Expression{Func{TSource, TMiddle}})"/>
    public static IQueryable<TResult> Select<TSource, TMiddle1, TMiddle2, TMiddle3, TResult>(
        this IQueryable<TSource> source,
        Expression<Func<TMiddle1, TMiddle2, TMiddle3, TResult>> selector,
        Expression<Func<TSource, TMiddle1>> middle1,
        Expression<Func<TSource, TMiddle2>> middle2,
        Expression<Func<TSource, TMiddle3>> middle3)
    {
        return source.Select(selector.LambdaConcatenation<TSource, TResult>(middle1, middle2, middle3));
    }

    /// <inheritdoc cref="Select{TSource, TMiddle, TResult}(IQueryable{TSource}, Expression{Func{TMiddle, TResult}}, Expression{Func{TSource, TMiddle}})"/>
    public static IQueryable<TResult> Select<TSource, TMiddle1, TMiddle2, TMiddle3, TMiddle4, TResult>(
        this IQueryable<TSource> source,
        Expression<Func<TMiddle1, TMiddle2, TMiddle3, TMiddle4, TResult>> selector,
        Expression<Func<TSource, TMiddle1>> middle1,
        Expression<Func<TSource, TMiddle2>> middle2,
        Expression<Func<TSource, TMiddle3>> middle3,
        Expression<Func<TSource, TMiddle4>> middle4)
    {
        return source.Select(selector.LambdaConcatenation<TSource, TResult>(middle1, middle2, middle3, middle4));
    }

    /// <inheritdoc cref="Select{TSource, TMiddle, TResult}(IQueryable{TSource}, Expression{Func{TMiddle, TResult}}, Expression{Func{TSource, TMiddle}})"/>
    public static IQueryable<TResult> Select<TSource, TMiddle1, TMiddle2, TMiddle3, TMiddle4, TMiddle5, TResult>(
        this IQueryable<TSource> source,
        Expression<Func<TMiddle1, TMiddle2, TMiddle3, TMiddle4, TMiddle5, TResult>> selector,
        Expression<Func<TSource, TMiddle1>> middle1,
        Expression<Func<TSource, TMiddle2>> middle2,
        Expression<Func<TSource, TMiddle3>> middle3,
        Expression<Func<TSource, TMiddle4>> middle4,
        Expression<Func<TSource, TMiddle5>> middle5)
    {
        return source.Select(selector.LambdaConcatenation<TSource, TResult>(middle1, middle2, middle3, middle4, middle5));
    }

    /// <inheritdoc cref="Select{TSource, TMiddle, TResult}(IQueryable{TSource}, Expression{Func{TMiddle, TResult}}, Expression{Func{TSource, TMiddle}})"/>
    public static IQueryable<TResult> Select<TSource, TMiddle1, TMiddle2, TMiddle3, TMiddle4, TMiddle5, TMiddle6, TResult>(
        this IQueryable<TSource> source,
        Expression<Func<TMiddle1, TMiddle2, TMiddle3, TMiddle4, TMiddle5, TMiddle6, TResult>> selector,
        Expression<Func<TSource, TMiddle1>> middle1,
        Expression<Func<TSource, TMiddle2>> middle2,
        Expression<Func<TSource, TMiddle3>> middle3,
        Expression<Func<TSource, TMiddle4>> middle4,
        Expression<Func<TSource, TMiddle5>> middle5,
        Expression<Func<TSource, TMiddle6>> middle6)
    {
        return source.Select(selector.LambdaConcatenation<TSource, TResult>(middle1, middle2, middle3, middle4, middle5, middle6));
    }
    #endregion
}
