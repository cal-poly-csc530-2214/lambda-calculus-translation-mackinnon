using LCTranslator.AST;

namespace LCTranslator.Analysis
{
    internal class Parser
    {
        private readonly Tokenizer _tokens;

        public Parser(string program)
        {
            _tokens = new Tokenizer(program);
        }

        public Expr ParseExpression()
            => ParseExpression(_tokens.GetNext());

        private Expr ParseExpression(string firstToken)
            => firstToken switch
            {
                "(" => ParseParenthesizedExpression(),
                var s when int.TryParse(s, out var i) => new NumExpr(i),
                var id => new IdExpr(id)
            };

        private Expr ParseParenthesizedExpression()
        {
            var firstToken = _tokens.GetNext();

            Expr expr = firstToken switch
            {
                "/" => ParseLambda(),
                "+" => ParseArith(ArithOp.Add),
                "*" => ParseArith(ArithOp.Multiply),
                "ifleq0" => ParseIfleq0(),
                "println" => ParsePrintln(),
                _ => ParseCall(firstToken),
            };

            _tokens.CheckNext(")");

            return expr;
        }

        private CallExpr ParseCall(string firstToken)
        {
            var func = ParseExpression(firstToken);
            var arg = ParseExpression();

            return new CallExpr(func, arg);
        }

        private LambdaExpr ParseLambda()
        {
            var id = new IdExpr(_tokens.GetNext());

            _tokens.CheckNext("=>");

            var body = ParseExpression();

            return new LambdaExpr(id, body);
        }

        private ArithExpr ParseArith(ArithOp operation)
        {
            var left = ParseExpression();
            var right = ParseExpression();

            return new ArithExpr(operation, left, right);
        }

        private Ifleq0Expr ParseIfleq0()
        {
            var operand = ParseExpression();
            var then = ParseExpression();
            var els = ParseExpression();

            return new Ifleq0Expr(operand, then, els);
        }

        private PrintlnExpr ParsePrintln()
        {
            var expr = ParseExpression();

            return new PrintlnExpr(expr);
        }
    }
}
