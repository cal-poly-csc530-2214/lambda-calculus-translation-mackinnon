using System.Collections.Generic;

namespace LCTranslator
{
    internal class Scope
    {
        private readonly Dictionary<string, IdExpr> _idsByName;

        public Scope()
        {
            _idsByName = new();
        }

        public Scope(Scope current)
        {
            _idsByName = new(current._idsByName);
        }

        public void AddIdExpr(IdExpr expr)
        {
            // It's ok if we overwrite an existing ID, that's why we have multiple scopes.
            _idsByName[expr.Id] = expr;
        }

        public IdExpr GetIdExpr(string id)
        {
            if (!_idsByName.TryGetValue(id, out var expr))
            {
                throw new LCException($"Invalid use of free variable '{id}'.");
            }

            return expr;
        }
    }
}
