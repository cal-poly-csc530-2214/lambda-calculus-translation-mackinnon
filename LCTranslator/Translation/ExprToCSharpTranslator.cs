using LCTranslator.AST;

namespace LCTranslator.Translation
{
    internal class ExprToCSharpTranslator : IExprVisitor<string>
    {
        private readonly TyToCSharpTranslator _typeTranslator = new();

        public string Translate(Expr expr)
            => expr.Accept(this);

        string IExprVisitor<string>.Visit(NumExpr e)
            => e.Num.ToString();

        string IExprVisitor<string>.Visit(IdExpr e)
            => e.Id;

        string IExprVisitor<string>.Visit(LambdaExpr e)
            => $"new {_typeTranslator.Translate(e.Type)}({e.IdExpr.Id} => {e.BodyExpr.Accept(this)})";

        string IExprVisitor<string>.Visit(CallExpr e)
            => $"{e.FuncExpr.Accept(this)}({e.ArgExpr.Accept(this)})";

        string IExprVisitor<string>.Visit(ArithExpr e)
            => $"{e.Left.Accept(this)} {e.Operation.AsChar()} {e.Right.Accept(this)}";

        string IExprVisitor<string>.Visit(Ifleq0Expr e)
            => e.Type switch
            {
                VoidTy => $"{{ if ({e.Operand.Accept(this)} <= 0) {{ {e.Then.Accept(this)}; }} else {{ {e.Else.Accept(this)}; }} }}",
                _ => $"{e.Operand.Accept(this)} <= 0 ? {e.Then.Accept(this)} : {e.Else.Accept(this)}"
            };

        string IExprVisitor<string>.Visit(PrintlnExpr e)
            => $"Console.WriteLine({e.Expr.Accept(this)})";
    }
}
