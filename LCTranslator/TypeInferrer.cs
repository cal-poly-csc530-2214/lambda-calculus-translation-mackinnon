namespace LCTranslator
{
    internal class TypeInferrer : IExprVisitor<Ty>
    {
        private Ty _expectedType = new UndefinedTy();

        public void SignalExpectedType(Expr target, Ty expectedType)
        {
            _expectedType = expectedType;

            target.Accept(this);
        }

        public Ty Visit(UndefinedExpr e)
            => new UndefinedTy();

        public Ty Visit(NumExpr e)
        {
            TryMergeTypes(e.Type, _expectedType);

            return e.Type;
        }

        public Ty Visit(IdExpr e)
        {
            e.SetType(TryMergeTypes(e.Type, _expectedType));

            return e.Type;
        }

        public Ty Visit(LambdaExpr e)
        {
            if (_expectedType is not FuncTy funcType)
            {
                throw new LCException($"Expected a function type, got '{_expectedType}'.");
            }

            _expectedType = funcType.ArgType;
            e.Id.Accept(this);

            _expectedType = funcType.ReturnType;
            e.Body.Accept(this);

            _expectedType = funcType;
            return e.Type;
        }

        public Ty Visit(CallExpr e)
        {
            var originalExpectedType = _expectedType;

            _expectedType = new FuncTy(new UndefinedTy(), originalExpectedType);
            e.Func.Accept(this);

            if (e.Func.Type is not FuncTy funcType)
            {
                throw new LCException("TODO: Make a helper in CallExpr to get the function type");
            }

            _expectedType = funcType.ArgType;
            e.Arg.Accept(this);

            _expectedType = new FuncTy(e.Arg.Type, new UndefinedTy());
            e.Func.Accept(this);

            _expectedType = originalExpectedType;

            return originalExpectedType;
        }

        public Ty Visit(ArithmeticExpr e)
        {
            TryMergeTypes(e.Type, _expectedType);

            return e.Type;
        }

        public Ty Visit(Ifleq0Expr e)
        {
            e.Then.Accept(this);
            e.Else.Accept(this);

            return e.Type;
        }

        public Ty Visit(PrintlnExpr e)
        {
            TryMergeTypes(e.Type, _expectedType);

            return e.Type;
        }

        private Ty TryMergeTypes(Ty originalType, Ty newType)
            => (originalType, newType) switch
            {
                (UndefinedTy, _) => newType,
                (_, UndefinedTy) => originalType,
                (FuncTy originalFunc, FuncTy newFunc) => new FuncTy(
                    TryMergeTypes(originalFunc.ArgType, newFunc.ArgType),
                    TryMergeTypes(originalFunc.ReturnType, newFunc.ReturnType)),
                _ when originalType == newType => originalType,
                _ => throw new LCException($"Encountered conflicting types '{originalType}' and '{newType}'.")
            };
    }
}
