using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Interpreter.Core.Execution.Objects.MagicMethods;

namespace Interpreter.Core.Execution.Objects
{
    public class Array : Obj, IIndexReadable, IIndexSettable
    {
        public Array(ImmutableArray<Obj> items) : base(items.ToArray())
        {
        }

        public override ObjectTypes Type => ObjectTypes.Array;

        private Obj[] Items => (Obj[])Value;
        
        public Obj this[int index]
        {
            get
            {
                index = NormalizeIndex(index);
                    
                if (!IsInBound(index))
                    throw new IndexOutOfRangeException();

                return Items[index];
            }

            set
            {
                index = NormalizeIndex(index);
                
                if (!IsInBound(index))
                    throw new IndexOutOfRangeException();

                Items[index] = value;
            }
        }

        public override string ToString()
        {
            var result = new StringBuilder();
            result.Append('[');

            for (var i = 0; i < Items.Length; i++)
            {
                var item = Items[i];
                
                result.Append(item.Type == ObjectTypes.String ? $"\"{item}\"" : item);

                if (i + 1 < Items.Length)
                    result.Append(", ");
            }
            
            result.Append(']');
            return result.ToString();
        }

        public override Boolean ToBoolean() => new(Items.Length > 0);

        private bool IsInBound(int index) => index >= 0 && index < Items.Length;

        private int NormalizeIndex(int index) => index >= 0 ? index : Items.Length + index;
    }
}