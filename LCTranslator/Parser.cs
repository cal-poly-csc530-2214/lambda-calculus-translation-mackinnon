namespace LCTranslator
{
    internal static class Parser
    {
        public static Expr ParseProgram(string program)
            => ParseExpression(new Tokenizer(program));

        private static Expr ParseExpression(Tokenizer tokens)
            => ParseExpression(tokens, tokens.GetNext());

        private static Expr ParseExpression(Tokenizer tokens, string firstToken)
            => firstToken switch
            {
                "(" => ParseParenthesizedExpression(tokens),
                var s when int.TryParse(s, out var i) => new NumExpr(i),
                var id => new IdExpr(id)
            };

        private static Expr ParseParenthesizedExpression(Tokenizer tokens)
        {
            var token = tokens.GetNext();

            Expr expr = token switch
            {
                "/" => ParseLambda(tokens),
                "+" => ParseArithmetic(tokens, ArithmeticOperation.Add),
                "*" => ParseArithmetic(tokens, ArithmeticOperation.Multiply),
                "ifleq0" => ParseIfleq0(tokens),
                "println" => ParsePrintln(tokens),
                _ => ParseCall(tokens, token),
            };

            tokens.CheckNext(")");

            return expr;
        }

        private static CallExpr ParseCall(Tokenizer tokens, string firstToken)
        {
            var func = ParseExpression(tokens, firstToken);
            var arg = ParseExpression(tokens);

            return new CallExpr(func, arg);
        }

        private static LambdaExpr ParseLambda(Tokenizer tokens)
        {
            var id = tokens.GetNext();

            tokens.CheckNext("=>");

            var body = ParseExpression(tokens);

            return new LambdaExpr(id, body);
        }

        private static ArithmeticExpr ParseArithmetic(Tokenizer reader, ArithmeticOperation operation)
        {
            var left = ParseExpression(reader);
            var right = ParseExpression(reader);

            return new ArithmeticExpr(operation, left, right);
        }

        private static Ifleq0Expr ParseIfleq0(Tokenizer reader)
        {
            var operand = ParseExpression(reader);
            var then = ParseExpression(reader);
            var els = ParseExpression(reader);

            return new Ifleq0Expr(operand, then, els);
        }

        private static PrintlnExpr ParsePrintln(Tokenizer reader)
        {
            var expr = ParseExpression(reader);

            return new PrintlnExpr(expr);
        }
    }
}
