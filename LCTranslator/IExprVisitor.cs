using LCTranslator.AST;

namespace LCTranslator
{
    internal interface IExprVisitor<T>
    {
        T Visit(NumExpr e);
        T Visit(IdExpr e);
        T Visit(LambdaExpr e);
        T Visit(CallExpr e);
        T Visit(ArithExpr e);
        T Visit(Ifleq0Expr e);
        T Visit(PrintlnExpr e);
    }

    internal interface IExprVisitor : IExprVisitor<object?>
    {
        new void Visit(NumExpr e);
        new void Visit(IdExpr e);
        new void Visit(LambdaExpr e);
        new void Visit(CallExpr e);
        new void Visit(ArithExpr e);
        new void Visit(Ifleq0Expr e);
        new void Visit(PrintlnExpr e);

        object? IExprVisitor<object?>.Visit(NumExpr e)
        {
            Visit(e);
            return null;
        } 

        object? IExprVisitor<object?>.Visit(IdExpr e)
        {
            Visit(e);
            return null;
        } 

        object? IExprVisitor<object?>.Visit(LambdaExpr e)
        {
            Visit(e);
            return null;
        } 

        object? IExprVisitor<object?>.Visit(CallExpr e)
        {
            Visit(e);
            return null;
        } 

        object? IExprVisitor<object?>.Visit(ArithExpr e)
        {
            Visit(e);
            return null;
        } 

        object? IExprVisitor<object?>.Visit(Ifleq0Expr e)
        {
            Visit(e);
            return null;
        } 

        object? IExprVisitor<object?>.Visit(PrintlnExpr e)
        {
            Visit(e);
            return null;
        } 
    }
}
