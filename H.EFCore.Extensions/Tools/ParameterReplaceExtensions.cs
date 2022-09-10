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
    /// <param name="expression">The expression to be changed</param>
    /// <param name="pairs">Replace dictionary, keys will be replaced with values.</param>
    /// <returns>New expression with the parameters replaced</returns>
    public static Expression ReplaceParameters(this Expression expression, Dictionary<ParameterExpression, Expression> pairs)
    {
        return new ParameterReplaceVisitor(pairs).Visit(expression);
    }

    ///<param name="from">Replaced Parameter</param>
    ///<param name="to">Replace target</param>
    /// <inheritdoc cref="ReplaceParameters(Expression, Dictionary{ParameterExpression, Expression})"/>
    public static Expression ReplaceParameter(this Expression expression, ParameterExpression from, Expression to)
    {
        return expression.ReplaceParameters(new() { [from] = to });
    }

    /// <inheritdoc cref="ReplaceParameter(Expression, ParameterExpression, Expression)"/>
    /// <remarks> <paramref name="from"/> and <paramref name="to"/> will be replaced in sequence, the excess will be skipped</remarks>
    public static Expression ReplaceParameters(this Expression expression, IEnumerable<ParameterExpression> from, IEnumerable<Expression> to)
    {
        var dic = from.Zip(to).ToDictionary(x => x.First, x => x.Second);
        return expression.ReplaceParameters(dic);
    }
}
