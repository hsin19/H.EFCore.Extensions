// Reference from https://github.com/DogusTeknoloji/BatMap/blob/master/BatMap/ParameterReplaceVisitor.cs

using System.Linq.Expressions;

namespace H.EFCore.Extensions.Tools;

internal class ParameterReplaceVisitor : ExpressionVisitor
{
    private readonly Dictionary<ParameterExpression, Expression> _pairs;

    internal ParameterReplaceVisitor(Dictionary<ParameterExpression, Expression> pairs)
    {
        if (pairs.Any(e => !e.Value.Type.IsAssignableTo(e.Key.Type)))
        {
            throw new ArgumentException($"Some of {nameof(pairs)} cannot be assigned.", nameof(pairs));
        }
        _pairs = pairs;
    }

    protected override Expression VisitParameter(ParameterExpression node)
    {
        return _pairs.TryGetValue(node, out var newPrm) ? newPrm : base.VisitParameter(node);
    }
}