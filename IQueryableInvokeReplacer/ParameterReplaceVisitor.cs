// Reference from https://github.com/DogusTeknoloji/BatMap/blob/master/BatMap/ParameterReplaceVisitor.cs

using System.Linq.Expressions;

namespace IQueryableInvokeReplacer;

internal class ParameterReplaceVisitor : ExpressionVisitor
{
    private readonly Dictionary<ParameterExpression, Expression> _pairs;

    internal ParameterReplaceVisitor(Dictionary<ParameterExpression, Expression> pairs)
    {
        _pairs = pairs;
    }

    protected override Expression VisitParameter(ParameterExpression node)
    {
        return _pairs.TryGetValue(node, out var newPrm) ? newPrm : base.VisitParameter(node);
    }
}