using System;
using System.Collections.Generic;

namespace LCTranslator
{
    public class Tokenizer
    {
        private static readonly string[] delimiterTokens = new[] { "(", ")" };

        private readonly Queue<string> tokens;

        public Tokenizer(string program)
        {
            foreach (var delimiterToken in delimiterTokens)
            {
                program = program.Replace(delimiterToken, $" {delimiterToken} ");
            }

            tokens = new(program.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
        }

        public string GetNext()
        {
            if (tokens.Count == 0)
            {
                throw new LCException("Unexpected end-of-file encountered.");
            }

            return tokens.Dequeue();
        }

        public void CheckNext(string expected)
        {
            string token = GetNext();

            if (token != expected)
            {
                throw new LCException($"Invalid token '{token}' (expected '{expected}').");
            }
        }
    }
}
