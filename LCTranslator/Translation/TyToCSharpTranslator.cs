using LCTranslator.AST;

namespace LCTranslator.Translation
{
    internal class TyToCSharpTranslator : ITyVisitor<string>
    {
        public string Translate(Ty type)
            => type.Accept(this);

        public string Visit(UndefinedTy ty)
            => throw LCErrors.UndefinedOrAmbiguousType();

        public string Visit(VoidTy ty)
            => "void";

        public string Visit(NumTy ty)
            => "int";

        public string Visit(FuncTy ty)
        {
            if (ty.ArgType is VoidTy)
            {
                throw LCErrors.VoidArgument();
            }

            if (ty.ReturnType is VoidTy)
            {
                return $"Action<{ty.ArgType.Accept(this)}>";
            }

            return $"Func<{ty.ArgType.Accept(this)}, {ty.ReturnType.Accept(this)}>";
        }
    }
}
