using System;

namespace LCTranslator
{
    internal class LCException : Exception
    {
        public LCException(string message) : base(message) { }
    }
}
