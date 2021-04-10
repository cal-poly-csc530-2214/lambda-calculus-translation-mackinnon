using System;
using System.Collections.Generic;

namespace LCTranslator.Analysis
{
    public class Tokenizer
    {
        private static readonly string[] _delimiterTokens = new[] { "(", ")" };

        private readonly Queue<string> _tokens;

        public Tokenizer(string program)
        {
            foreach (var delimiterToken in _delimiterTokens)
            {
                program = program.Replace(delimiterToken, $" {delimiterToken} ");
            }

            _tokens = new(program.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
        }

        public string GetNext()
        {
            if (_tokens.Count == 0)
            {
                throw new LCException("Unexpected end-of-file encountered.");
            }

            return _tokens.Dequeue();
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
