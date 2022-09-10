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
    /// <param name="parameter">The compared object</param>
    /// <param name="obj">The comparison object</param>
    /// <param name="keys">Compare property sets</param>
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

    /// <summary>
    /// Creates a <see cref="MemberExpression"/> by the complex property name.
    /// </summary>
    /// <param name="name">Complex property name</param>
    /// <inheritdoc cref="Expression.Property(Expression, string)"/>
    public static MemberExpression? GetComplexProperty(this Expression expression, string name)
    {
        var properties = expression.Type.GetComplexProperty(name);
        if (properties == null)
        {
            return null;
        }
        MemberExpression? ret = null;
        foreach (var property in properties)
        {
            ret = Expression.Property(ret ?? expression, property);
        }
        return ret;
    }

    #region default method extension

    /// <remarks>If either <paramref name="left"/> or <paramref name="right"/> is <see langword="null"/>, return <c><paramref name="left"/> ?? <paramref name="right"/></c></remarks>
    /// <inheritdoc cref="Expression.AndAlso(Expression, Expression)"/>
    public static Expression? AndAlso(this Expression? left, Expression? right)
    {
        return left is null || right is null ? left ?? right : Expression.AndAlso(left, right);
    }

    /// <remarks>If either <paramref name="left"/> or <paramref name="right"/> is <see langword="null"/>, return <c><paramref name="left"/> ?? <paramref name="right"/></c></remarks>
    /// <inheritdoc cref="Expression.OrElse(Expression, Expression)"/>
    public static Expression? OrElse(this Expression? left, Expression? right)
    {
        return left is null || right is null ? left ?? right : Expression.OrElse(left, right);
    }


    /// <summary>
    /// Use or( <see langword="||"/> ) connect all conditions
    /// </summary>
    /// <param name="conditions">Condition set</param>
    /// <inheritdoc cref="Expression.OrElse(Expression, Expression)"/>
    public static Expression OrElse(params Expression[] conditions)
    {
        switch (conditions.Length)
        {
            case 0:
                throw new ArgumentException("Array cannot be empty.", nameof(conditions));
            case 1:
                return conditions[0];
            case 2:
                return Expression.OrElse(conditions[0], conditions[1]);
            default:
            {
                var spiltIndex = conditions.Length / 2;
                return Expression.OrElse(OrElse(conditions[..spiltIndex]), OrElse(conditions[spiltIndex..]));
            }
        }
    }

    #endregion
}
