using System.Linq.Expressions;

namespace IQueryableInvokeReplacer;

internal class InvokeReplacerVistor : ExpressionVisitor
{
    private readonly LambdaExpression lambaExpress;

    public InvokeReplacerVistor(LambdaExpression func)
    {
        lambaExpress = func;
    }

    protected override Expression VisitLambda<T>(Expression<T> node)
    {
        if (node.Body is MethodCallExpression methodCall && methodCall.Method.Name == "Invoke")
        {
            if (methodCall.Method.ReturnType == lambaExpress.ReturnType
                && methodCall.Arguments.Select(p => p.Type).SequenceEqual(lambaExpress.Parameters.Select(p => p.Type)))
            {
                var newBody = lambaExpress.Body.ReplaceParameters(lambaExpress.Parameters, methodCall.Arguments);
                return node.Update(newBody, node.Parameters);
            }
        }
        return base.VisitLambda(node);
    }
}
