using System.Linq.Expressions;
using System.Reflection;

namespace IQuerableExtensions;

public static class ExpressionExtensions
{

    /// <summary>
    /// Get Condition Expression
    /// </summary>
    /// <returns> <c><paramref name="parameter"/>.key1 == <paramref name="obj"/>.key1 And <paramref name="parameter"/>.key2 == ... </c> </returns>
    public static Expression? GetEquelCondition(this ParameterExpression parameter, object? obj, IEnumerable<PropertyInfo> keys)
    {
        if (obj == null || !keys.Any())
            return null;
        var entityConst = Expression.Constant(obj);
        Expression? condition = null;
        foreach (var key in keys)
        {
            var paramEquals = Expression.Equal(Expression.Property(parameter, key.Name), Expression.Property(entityConst, key.Name));
            condition = condition.AndAlso(paramEquals);
        }
        return condition;
    }

    public static Expression? AndAlso(this Expression? left, Expression? right)
    {
        if (left == null)
            return right;
        else if (right != null)
            return Expression.AndAlso(left, right);
        else return null;
    }

    public static Expression? OrElse(this Expression? left, Expression? right)
    {
        if (left == null)
            return right;
        else if (right != null)
            return Expression.OrElse(left, right);
        else return null;
    }
}
