namespace LCTranslator
{
    internal interface IExprVisitor<T>
    {
        T Visit(NumExpr e);
        T Visit(IdExpr e);
        T Visit(LambdaExpr e);
        T Visit(CallExpr e);
        T Visit(ArithmeticExpr e);
        T Visit(Ifleq0Expr e);
        T Visit(PrintlnExpr e);
    }
}
