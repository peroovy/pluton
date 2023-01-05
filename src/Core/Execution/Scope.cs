using System;
using System.Collections.Generic;
using Core.Execution.Objects;
using Core.Lexing;

namespace Core.Execution
{
    public class Scope
    {
        private readonly Dictionary<string, Obj> identifiers = new();
        
        public Scope(Scope parent)
        {
            Parent = parent;
        }
        
        public Scope Parent { get; }

        public bool Contains(string identifier) => identifiers.ContainsKey(identifier);

        public void Assign(string identifier, Obj value) => identifiers[identifier] = value;

        public bool TryLookup(string identifier, out Obj value)
        {
            if (identifiers.TryGetValue(identifier, out value))
                return true;

            return Parent?.TryLookup(identifier, out value) ?? false;
        }

        public Obj Lookup(string identifier)
        {
            if (!TryLookup(identifier, out var value))
                throw new ArgumentException($"Unknown identifier '{identifier}'");

            return value;
        }
    }
}