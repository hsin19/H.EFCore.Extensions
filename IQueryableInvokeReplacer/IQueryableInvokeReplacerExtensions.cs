using System.Linq.Expressions;

namespace IQueryableInvokeReplacer;

public static class ExpressionVisitorExtensions
{
    /// <summary>
    /// Replace <see cref="Func{}.Invoke"/> to <paramref name="lambaExpress"/> if they have the same type of input and output
    /// </summary>
    public static IQueryable<T> ReplaceInvokeFunction<T>(this IQueryable<T> souce, LambdaExpression lambaExpress)
    {
        var expressionTree = new InvokeReplacerVistor(lambaExpress).Visit(souce.Expression);
        return souce.Provider.CreateQuery<T>(expressionTree);
    }

    public static Expression ReplaceParameter(this Expression expression, ParameterExpression from, Expression to)
    {
        if (!to.Type.IsAssignableTo(from.Type))
            throw new ArgumentException($"{nameof(to)} can not be assigned to {from.Type.Name}", nameof(to));
        return ReplaceParameters(expression, new() { [from] = to });
    }

    public static Expression ReplaceParameters(this Expression expression, IEnumerable<ParameterExpression> from, IEnumerable<Expression> to)
    {
        var dic = from.Zip(to)
                      .Where(e => e.Second.Type.IsAssignableTo(e.First.Type))
                      .ToDictionary(x => x.First, x => x.Second);
        return ReplaceParameters(expression, dic);
    }

    public static Expression ReplaceParameters(this Expression expression, Dictionary<ParameterExpression, Expression> pairs)
    {
        return new ParameterReplaceVisitor(pairs).Visit(expression);
    }
}
