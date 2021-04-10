using LCTranslator.AST;

namespace LCTranslator.Translation
{
    internal class TyToLCTranslator : ITyVisitor<string>
    {
        public string Translate(Ty type)
            => type.Accept(this);

        string ITyVisitor<string>.Visit(UndefinedTy ty)
            => "undefined";

        string ITyVisitor<string>.Visit(VoidTy ty)
            => "void";

        string ITyVisitor<string>.Visit(NumTy ty)
            => "num";

        string ITyVisitor<string>.Visit(FuncTy ty)
            => $"({ty.ArgType.Accept(this)} -> {ty.ReturnType.Accept(this)})";
    }
}
