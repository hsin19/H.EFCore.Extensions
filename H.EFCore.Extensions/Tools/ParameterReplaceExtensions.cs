using System.Linq.Expressions;

namespace H.EFCore.Extensions.Tools;

/// <summary>
/// Replace the specified Parameter Expression
/// </summary>
public static class ParameterReplaceExtensions
{
    /// <summary>
    /// Replace the specified <see cref="ParameterExpression" /> to <see cref="Expression" />
    /// </summary>
    /// <returns></returns>
    public static Expression ReplaceParameters(this Expression expression, Dictionary<ParameterExpression, Expression> pairs)
    {
        return new ParameterReplaceVisitor(pairs).Visit(expression);
    }

    /// <inheritdoc cref="ReplaceParameters(Expression, Dictionary{ParameterExpression, Expression})"/>
    public static Expression ReplaceParameter(this Expression expression, ParameterExpression from, Expression to)
    {
        return expression.ReplaceParameters(new() { [from] = to });
    }

    /// <inheritdoc cref="ReplaceParameters(Expression, Dictionary{ParameterExpression, Expression})"/>
    public static Expression ReplaceParameters(this Expression expression, IEnumerable<ParameterExpression> from, IEnumerable<Expression> to)
    {
        var dic = from.Zip(to).ToDictionary(x => x.First, x => x.Second);
        return expression.ReplaceParameters(dic);
    }
}
