using LCTranslator.AST;
using LCTranslator.Translation;

namespace LCTranslator
{
    internal static class LCErrors
    {
        private static readonly TyToLCTranslator _tyTranslator = new();

        public static LCException FreeVariable(string id)
            => new($"Invalid use of free variable '{id}'.");

        public static LCException UnexpectedEndOfFile()
            => new("Unexpected end-of-file encountered.");

        public static LCException InvalidToken(string token, string expected)
            => new($"Invalid token '{token}' (expected '{expected}').");

        public static LCException Ifleq0TypeMismatch(Ty thenType, Ty elseType)
            => new($"The cases of an 'ifleq0' had mismatching types " +
                   $"'{_tyTranslator.Translate(thenType)}' and " +
                   $"'{_tyTranslator.Translate(elseType)}'.");

        public static LCException ValuesCannotBeVoid()
            => new($"A value cannot be assigned type '{_tyTranslator.Translate(new VoidTy())}'.");

        public static LCException UnexpectedType(Ty expectedType, Ty actualType)
            => new($"Expected type '{_tyTranslator.Translate(expectedType)}', " +
                   $"got '{_tyTranslator.Translate(actualType)}'.");

        public static LCException ConflictingTypes(Ty originalType, Ty newType)
            => new($"Encountered conflicting types " +
                   $"'{_tyTranslator.Translate(originalType)}' and " +
                   $"'{_tyTranslator.Translate(newType)}'.");

        public static LCException UndefinedOrAmbiguousType()
            => new("Part of the program had an undefined or ambiguous type.");

        public static LCException VoidArgument()
            => new($"An argument cannot be of type '{_tyTranslator.Translate(new VoidTy())}'.");
    }
}
