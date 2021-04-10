namespace LCTranslator.AST
{
    internal abstract record Ty
    {
        public abstract T Accept<T>(ITyVisitor<T> visitor);
    }

    internal record UndefinedTy : Ty
    {
        public override T Accept<T>(ITyVisitor<T> visitor) => visitor.Visit(this);
    }

    internal record VoidTy : Ty
    {
        public override T Accept<T>(ITyVisitor<T> visitor) => visitor.Visit(this);
    }

    internal record NumTy : Ty
    {
        public override T Accept<T>(ITyVisitor<T> visitor) => visitor.Visit(this);
    }

    internal record FuncTy : Ty
    {
        public Ty ArgType { get; set; } = new UndefinedTy();
        public Ty ReturnType { get; set; } = new UndefinedTy();

        public override T Accept<T>(ITyVisitor<T> visitor) => visitor.Visit(this);
    }
}
