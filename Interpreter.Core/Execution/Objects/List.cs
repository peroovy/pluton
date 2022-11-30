using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Interpreter.Core.Execution.Objects.MagicMethods;

namespace Interpreter.Core.Execution.Objects
{
    public class List : Obj, IReadIndex
    {
        public List(ImmutableArray<Obj> items) : base(items.ToList())
        {
        }
        
        public override ObjectTypes Type => ObjectTypes.List;

        private List<Obj> Items => (List<Obj>)Value;
        
        public Obj this[int index]
        {
            get
            {
                if (!IsInBound(index))
                    throw new IndexOutOfRangeException();

                return Items[index];
            }

            set
            {
                if (!IsInBound(index))
                    throw new IndexOutOfRangeException();

                Items[index] = value;
            }
        }

        public override string ToString()
        {
            var result = new StringBuilder();
            result.Append('[');

            for (var i = 0; i < Items.Count; i++)
            {
                var item = Items[i];
                
                if (item.Type == ObjectTypes.String)
                {
                    result.Append($"\"{item}\"");
                    continue;
                }

                result.Append(item);
                if (i + 1 < Items.Count)
                    result.Append(", ");
            }
            
            result.Append(']');
            return result.ToString();
        }

        public override Boolean ToBoolean() => new(Items.Count > 0);

        private bool IsInBound(int index) => index >= 0 && index < Items.Count;
    }
}