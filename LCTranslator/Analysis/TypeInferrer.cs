using LCTranslator.AST;

namespace LCTranslator.Analysis
{
    internal class TypeInferrer : IExprVisitor
    {
        private readonly Scope _scope = new();

        private Ty _expectedType = new UndefinedTy();

        public void InferTypes(Expr program)
            => ExpectType(program, new UndefinedTy(), true);

        void IExprVisitor.Visit(NumExpr e)
        {
            e.Type = CastType<NumTy>(_expectedType);
        }

        void IExprVisitor.Visit(IdExpr e)
        {
            var node = _scope.GetVariable(e.Id);
            var type = MergeTypes(node.Value.type, _expectedType);

            e.Type = type;
            node.Value = (e.Id, type);
        }

        void IExprVisitor.Visit(LambdaExpr e)
        {
            var funcType = CastType<FuncTy>(_expectedType);

            _scope.PushVariable(e.IdExpr.Id);

            ExpectType(e.IdExpr, funcType.ArgType);
            ExpectType(e.BodyExpr, funcType.ReturnType, true);

            var idType = _scope.PopVariable();

            e.IdExpr.Type = MergeTypes(e.IdExpr.Type, idType);
            e.Type = MergeTypes(funcType, new FuncTy
            {
                ArgType = e.IdExpr.Type,
                ReturnType = e.BodyExpr.Type
            });
        }

        void IExprVisitor.Visit(CallExpr e)
        {
            ExpectType(e.FuncExpr, new FuncTy { ReturnType = _expectedType });

            var funcType = CastType<FuncTy>(e.FuncExpr.Type);

            ExpectType(e.ArgExpr, funcType.ArgType);
            ExpectType(e.FuncExpr, new FuncTy { ArgType = e.ArgExpr.Type });

            funcType = CastType<FuncTy>(e.FuncExpr.Type);

            e.Type = funcType.ReturnType;
        }

        void IExprVisitor.Visit(ArithExpr e)
        {
            e.Type = CastType<NumTy>(_expectedType);

            ExpectType(e.Left, new NumTy());
            ExpectType(e.Right, new NumTy());
        }

        void IExprVisitor.Visit(Ifleq0Expr e)
        {
            e.Then.Accept(this);
            e.Else.Accept(this);

            ExpectType(e.Operand, new NumTy());

            if (e.Then.Type != e.Else.Type)
            {
                throw LCErrors.Ifleq0TypeMismatch(e.Then.Type, e.Else.Type);
            }

            e.Type = e.Then.Type;
        }

        void IExprVisitor.Visit(PrintlnExpr e)
        {
            e.Type = CastType<VoidTy>(_expectedType);

            ExpectType(e.Expr, new UndefinedTy());
        }

        private void ExpectType(Expr expr, Ty expectedType, bool canBeVoid = false)
        {
            var lastExpectedType = _expectedType;
            _expectedType = expectedType;

            expr.Accept(this);

            if (!canBeVoid && expr.Type is VoidTy)
            {
                throw LCErrors.ValuesCannotBeVoid();
            }

            _expectedType = lastExpectedType;
        }

        private T CastType<T>(Ty type) where T : Ty, new()
        {
            if (type is UndefinedTy)
            {
                return new T();
            }

            if (type is not T result)
            {
                throw LCErrors.UnexpectedType(_expectedType, new T());
            }

            return result;
        }

        private Ty MergeTypes(Ty originalType, Ty newType)
            => (originalType, newType) switch
            {
                (UndefinedTy, _) => newType,
                (_, UndefinedTy) => originalType,
                (FuncTy originalFunc, FuncTy newFunc) => new FuncTy
                {
                    ArgType = MergeTypes(originalFunc.ArgType, newFunc.ArgType),
                    ReturnType = MergeTypes(originalFunc.ReturnType, newFunc.ReturnType),
                },
                _ when originalType == newType => originalType,
                _ => throw LCErrors.ConflictingTypes(originalType, newType)
            };
    }
}
