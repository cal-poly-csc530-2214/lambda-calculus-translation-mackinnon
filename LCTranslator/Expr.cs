using System;

namespace LCTranslator
{
    internal abstract record Expr
    {
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

    internal record LambdaExpr(string Id, Expr Body) : Expr
    {
        public override T Accept<T>(IExprVisitor<T> visitor) => visitor.Visit(this);
    }

    internal record CallExpr(Expr Func, Expr Arg) : Expr
    {
        public override T Accept<T>(IExprVisitor<T> visitor) => visitor.Visit(this);
    }

    internal record ArithmeticExpr(ArithmeticOperation Operation, Expr Left, Expr Right) : Expr
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

    internal enum ArithmeticOperation { Add, Multiply }

    internal static class ArithmeticOperationExtensions
    {
        public static char AsChar(this ArithmeticOperation operation) => operation switch
        {
            ArithmeticOperation.Add => '+',
            ArithmeticOperation.Multiply => '*',
            _ => throw new ArgumentException($"Unknown operation '{operation}'.", nameof(operation))
        };
    }
}
