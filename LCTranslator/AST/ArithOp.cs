using System;

namespace LCTranslator.AST
{
    internal enum ArithOp { Add, Multiply }

    internal static class ArithOpExtensions
    {
        public static char AsChar(this ArithOp operation) => operation switch
        {
            ArithOp.Add => '+',
            ArithOp.Multiply => '*',
            _ => throw new ArgumentException($"Unknown arithmetic operation '{operation}'.", nameof(operation))
        };
    }
}
