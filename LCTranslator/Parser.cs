namespace LCTranslator
{
    internal class Parser
    {
        private readonly Tokenizer _tokens;
        private readonly TypeInferrer _typeInferrer = new();

        public Parser(string program)
        {
            _tokens = new Tokenizer(program);
        }

        public Expr ParseProgram() => ParseExpression(new Scope());

        private Expr ParseExpression(Scope scope) => ParseExpression(scope, _tokens.GetNext());

        private Expr ParseExpression(Scope scope, string firstToken)
            => firstToken switch
            {
                "(" => ParseParenthesizedExpression(scope),
                var s when int.TryParse(s, out var i) => new NumExpr(i),
                var id => scope.GetIdExpr(id)
            };

        private Expr ParseParenthesizedExpression(Scope scope)
        {
            var firstToken = _tokens.GetNext();

            Expr expr = firstToken switch
            {
                "/" => ParseLambda(scope),
                "+" => ParseArithmetic(scope, ArithmeticOperation.Add),
                "*" => ParseArithmetic(scope, ArithmeticOperation.Multiply),
                "ifleq0" => ParseIfleq0(scope),
                "println" => ParsePrintln(scope),
                _ => ParseCall(scope, firstToken),
            };

            _tokens.CheckNext(")");

            return expr;
        }

        private CallExpr ParseCall(Scope scope, string firstToken)
        {
            var func = ParseExpression(scope, firstToken);
            var arg = ParseExpression(scope);

            _typeInferrer.SignalExpectedType(func, new FuncTy(arg.Type, new UndefinedTy()));

            return new CallExpr(func, arg);
        }

        private LambdaExpr ParseLambda(Scope scope)
        {
            var id = new IdExpr(_tokens.GetNext());

            _tokens.CheckNext("=>");

            var lambdaScope = new Scope(scope);
            lambdaScope.AddIdExpr(id); // We can't infer the type of the id until we use it.

            var body = ParseExpression(lambdaScope);

            return new LambdaExpr(id, body);
        }

        private ArithmeticExpr ParseArithmetic(Scope scope, ArithmeticOperation operation)
        {
            var left = ParseExpression(scope);
            var right = ParseExpression(scope);

            _typeInferrer.SignalExpectedType(left, new NumTy());
            _typeInferrer.SignalExpectedType(right, new NumTy());

            return new ArithmeticExpr(operation, left, right);
        }

        private Ifleq0Expr ParseIfleq0(Scope scope)
        {
            var operand = ParseExpression(scope);
            var then = ParseExpression(scope);
            var els = ParseExpression(scope);

            _typeInferrer.SignalExpectedType(operand, new NumTy());

            return new Ifleq0Expr(operand, then, els);
        }

        private PrintlnExpr ParsePrintln(Scope scope)
        {
            var expr = ParseExpression(scope);

            _typeInferrer.SignalExpectedType(expr, new NumTy());

            return new PrintlnExpr(expr);
        }
    }
}
