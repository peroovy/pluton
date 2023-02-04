using System;
using System.Collections.Generic;
using System.Linq;
using Core.Execution.Objects;

namespace Core.Execution
{
    public class Scope
    {
        private readonly Dictionary<string, Obj> names = new();
        
        public Scope(Scope parent)
        {
            Parent = parent;
        }
        
        public Scope Parent { get; }

        public IEnumerable<KeyValuePair<string, Obj>> CurrentLevel => names.AsEnumerable();

        public bool Contains(string name) => names.ContainsKey(name);

        public void Assign(string name, Obj value) => names[name] = value;

        public bool TryLookup(string name, out Obj value)
        {
            if (names.TryGetValue(name, out value))
                return true;

            return Parent?.TryLookup(name, out value) ?? false;
        }

        public Obj Lookup(string name)
        {
            if (!TryLookup(name, out var value))
                throw new ArgumentException($"Unknown name '{name}'");

            return value;
        }
    }
}