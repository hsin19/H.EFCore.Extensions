using System.Linq.Expressions;
using System.Reflection;

namespace H.EFCore.Extensions.Tools;

/// <summary>
/// Extension Method for <see cref="Expression"/>
/// </summary>
public static class ExpressionExtensions
{
    /// <summary>
    /// Get Condition Expression
    /// </summary>
    /// <returns> <c><paramref name="parameter"/>.key1 == <paramref name="obj"/>.key1 And <paramref name="parameter"/>.key2 == ... </c> </returns>
    public static Expression? GetEqualCondition(this ParameterExpression parameter, object? obj, IEnumerable<PropertyInfo> keys)
    {
        if (obj == null || !keys.Any())
        {
            return null;
        }

        var entityConst = Expression.Constant(obj);
        Expression? condition = null;
        foreach (var key in keys)
        {
            var paramEquals = Expression.Equal(Expression.Property(parameter, key.Name), Expression.Property(entityConst, key.Name));
            condition = condition.AndAlso(paramEquals);
        }
        return condition;
    }

    /// <remarks>If either <paramref name="left"/> or <paramref name="right"/> is <see langword="null"/>, return <c><paramref name="left"/> ?? <paramref name="right"/></c></remarks>
    /// <inheritdoc cref="Expression.AndAlso(Expression, Expression)"/>
    public static Expression? AndAlso(this Expression? left, Expression? right)
    {
        if (left is null || right is null)
            return left ?? right;
        else
            return Expression.AndAlso(left, right);
    }

    /// <remarks>If either <paramref name="left"/> or <paramref name="right"/> is <see langword="null"/>, return <c><paramref name="left"/> ?? <paramref name="right"/></c></remarks>
    /// <inheritdoc cref="Expression.OrElse(Expression, Expression)"/>
    public static Expression? OrElse(this Expression? left, Expression? right)
    {
        if (left is null || right is null)
            return left ?? right;
        else
            return Expression.OrElse(left, right);
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

    /// <summary>
    /// Replace the specified <see cref="ParameterExpression" /> to <see cref="Expression" />
    /// </summary>
    /// <returns></returns>
    public static Expression ReplaceParameters(this Expression expression, Dictionary<ParameterExpression, Expression> pairs)
    {
        return new ParameterReplaceVisitor(pairs).Visit(expression);
    }
}
