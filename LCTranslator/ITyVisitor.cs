using LCTranslator.AST;

namespace LCTranslator
{
    internal interface ITyVisitor<T>
    {
        T Visit(UndefinedTy ty);
        T Visit(VoidTy ty);
        T Visit(NumTy ty);
        T Visit(FuncTy ty);
    }
}
