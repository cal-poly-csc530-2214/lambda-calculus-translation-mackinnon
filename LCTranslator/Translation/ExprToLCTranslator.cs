using LCTranslator.AST;

namespace LCTranslator.Translation
{
    internal class ExprToLCTranslator : IExprVisitor<string>
    {
        private readonly TyToLCTranslator _tyTranslator = new();

        public string Translate(Expr e)
            => e.Accept(this);

        string IExprVisitor<string>.Visit(NumExpr e)
            => e.Num.ToString();

        string IExprVisitor<string>.Visit(IdExpr e)
            => e.Id;

        string IExprVisitor<string>.Visit(LambdaExpr e)
            => $"(/ {_tyTranslator.Translate(e.Type)} {e.IdExpr.Accept(this)} => {e.BodyExpr.Accept(this)})";

        string IExprVisitor<string>.Visit(CallExpr e)
            => $"({e.FuncExpr.Accept(this)} {e.ArgExpr.Accept(this)})";

        string IExprVisitor<string>.Visit(ArithExpr e)
            => $"({e.Operation.AsChar()} {e.Left.Accept(this)} {e.Right.Accept(this)})";

        string IExprVisitor<string>.Visit(Ifleq0Expr e)
            => $"(ifleq0 {e.Operand.Accept(this)} {e.Then.Accept(this)} {e.Else.Accept(this)})";

        string IExprVisitor<string>.Visit(PrintlnExpr e)
            => $"(println {e.Expr.Accept(this)})";
    }
}
