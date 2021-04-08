namespace LCTranslator.Translators
{
    internal class ExprToLCTranslator : IExprVisitor<string>
    {
        public string Translate(Expr e)
            => e.Accept(this);

        string IExprVisitor<string>.Visit(NumExpr e)
            => e.Num.ToString();

        string IExprVisitor<string>.Visit(IdExpr e)
            => e.Id;

        string IExprVisitor<string>.Visit(LambdaExpr e)
            => $"(/ {e.Id} => {e.Body.Accept(this)})";

        string IExprVisitor<string>.Visit(CallExpr e)
            => $"({e.Func.Accept(this)} {e.Arg.Accept(this)})";

        string IExprVisitor<string>.Visit(ArithmeticExpr e)
            => $"({e.Operation.AsChar()} {e.Left.Accept(this)} {e.Right.Accept(this)})";

        string IExprVisitor<string>.Visit(Ifleq0Expr e)
            => $"(ifleq0 {e.Operand.Accept(this)} {e.Then.Accept(this)} {e.Else.Accept(this)})";

        string IExprVisitor<string>.Visit(PrintlnExpr e)
            => $"(println {e.Expr.Accept(this)})";
    }
}
