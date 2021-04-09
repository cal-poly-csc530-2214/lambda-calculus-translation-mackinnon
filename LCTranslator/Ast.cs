using System;

namespace LCTranslator
{
    internal abstract record Ty;

    internal record UndefinedTy : Ty;

    internal record VoidTy : Ty;

    internal record NumTy : Ty;

    internal record FuncTy(Ty ArgType, Ty ReturnType) : Ty;

    internal abstract record Expr
    {
        public abstract Ty Type { get; }

        public abstract T Accept<T>(IExprVisitor<T> visitor);
    }

    internal record UndefinedExpr : Expr
    {
        public override Ty Type { get; } = new UndefinedTy();

        public override T Accept<T>(IExprVisitor<T> visitor) => visitor.Visit(this);
    }

    internal record NumExpr(int Num) : Expr
    {
        public override Ty Type { get; } = new NumTy();

        public override T Accept<T>(IExprVisitor<T> visitor) => visitor.Visit(this);
    }

    internal record IdExpr(string Id) : Expr
    {
        private Ty _type = new UndefinedTy();

        public override Ty Type => _type;

        public void SetType(Ty type) => _type = type;

        public override T Accept<T>(IExprVisitor<T> visitor) => visitor.Visit(this);
    }

    internal record LambdaExpr(IdExpr Id, Expr Body) : Expr
    {
        public override Ty Type => new FuncTy(Id.Type, Body.Type);

        public override T Accept<T>(IExprVisitor<T> visitor) => visitor.Visit(this);
    }

    internal record CallExpr(Expr Func, Expr Arg) : Expr
    {
        public override Ty Type => Func.Type is FuncTy funcTy
            ? funcTy.ReturnType
            : throw new LCException("Can only call function types.");//new UndefinedTy();

        public override T Accept<T>(IExprVisitor<T> visitor) => visitor.Visit(this);
    }

    internal record ArithmeticExpr(ArithmeticOperation Operation, Expr Left, Expr Right) : Expr
    {
        public override Ty Type { get; } = new NumTy();

        public override T Accept<T>(IExprVisitor<T> visitor) => visitor.Visit(this);
    }

    internal record Ifleq0Expr(Expr Operand, Expr Then, Expr Else) : Expr//(Then.GetBestMatchingType(Else))
    {
        // TODO: Type check later.
        public override Ty Type => Then.Type;

        public override T Accept<T>(IExprVisitor<T> visitor) => visitor.Visit(this);
    }

    internal record PrintlnExpr(Expr Expr) : Expr
    {
        public override Ty Type { get; } = new VoidTy();

        public override T Accept<T>(IExprVisitor<T> visitor) => visitor.Visit(this);
    }

    internal enum ArithmeticOperation { Add, Multiply }

    internal static class ExprExtensions
    {
        public static Ty GetFuncReturnType(this Expr expr)
            => expr.Type is FuncTy funcType
            ? funcType.ReturnType
            : throw new LCException("Cannot call a non-lambda.");

        public static Ty GetBestMatchingType(this Expr expr, Expr other)
        {
            if (expr.Type != other.Type)
            {
                throw new LCException("Could not find the best matching type for two expressions.");
            }

            return expr.Type;
        }
    }

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
