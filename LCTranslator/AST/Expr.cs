namespace LCTranslator.AST
{
    internal abstract record Expr
    {
        public Ty Type { get; set; } = new UndefinedTy();

        public abstract T Accept<T>(IExprVisitor<T> visitor);
    }

    internal record NumExpr(int Num) : Expr
    {
        public override T Accept<T>(IExprVisitor<T> visitor) => visitor.Visit(this);
    }

    internal record IdExpr(string Id) : Expr
    {
        public override T Accept<T>(IExprVisitor<T> visitor) => visitor.Visit(this);
    }

    internal record LambdaExpr(IdExpr IdExpr, Expr BodyExpr) : Expr
    {
        public override T Accept<T>(IExprVisitor<T> visitor) => visitor.Visit(this);
    }

    internal record CallExpr(Expr FuncExpr, Expr ArgExpr) : Expr
    {
        public override T Accept<T>(IExprVisitor<T> visitor) => visitor.Visit(this);
    }

    internal record ArithExpr(ArithOp Operation, Expr Left, Expr Right) : Expr
    {
        public override T Accept<T>(IExprVisitor<T> visitor) => visitor.Visit(this);
    }

    internal record Ifleq0Expr(Expr Operand, Expr Then, Expr Else) : Expr
    {
        public override T Accept<T>(IExprVisitor<T> visitor) => visitor.Visit(this);
    }

    internal record PrintlnExpr(Expr Expr) : Expr
    {
        public override T Accept<T>(IExprVisitor<T> visitor) => visitor.Visit(this);
    }
}
