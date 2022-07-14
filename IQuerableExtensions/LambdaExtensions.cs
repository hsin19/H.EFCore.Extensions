using System.Linq.Expressions;

namespace IQuerableExtensions;

public static class LambdaExtensions
{
    /// <summary>
    /// Lambda to Lambda <br/>
    /// (<typeparamref name="TSource"/>) => (<paramref name="paramters"/>) => (<typeparamref name="TResult"/>)
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
}
