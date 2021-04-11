using LCTranslator.AST;
using System;
using System.Collections.Generic;

namespace LCTranslator.Analysis
{
    internal class Scope
    {
        private readonly LinkedList<(string id, Ty type)> _stack = new();

        public void PushVariable(string id)
        {
            _stack.AddLast((id, new UndefinedTy()));
        }

        public Ty PopVariable()
        {
            var type = _stack.Last?.Value.type
                ?? throw new InvalidOperationException("Attempted to pop from an empty scope.");

            _stack.RemoveLast();

            return type;
        }

        public LinkedListNode<(string id, Ty type)> GetVariable(string id)
        {
            for (var node = _stack.Last; node != null; node = node.Previous)
            {
                if (node.Value.id == id)
                {
                    return node;
                }
            }

            throw LCErrors.FreeVariable(id);
        }
    }
}
