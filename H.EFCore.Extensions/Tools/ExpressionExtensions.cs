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

    #endregion
}
