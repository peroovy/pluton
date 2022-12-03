using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Interpreter.Core.Execution.Objects.MagicMethods;

namespace Interpreter.Core.Execution.Objects
{
    public class Array : Obj, IIndexReadable, IIndexSettable
    {
        public Array(ImmutableArray<Obj> items)
        {
            Items = items.ToArray();
        }

        private Array(Obj[] items)
        {
            Items = items;
        }
        
        private Obj[] Items { get; }
        
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
                
                result.Append(item is String ? $"\"{item}\"" : item);

                if (i + 1 < Items.Length)
                    result.Append(", ");
            }
            
            result.Append(']');
            return result.ToString();
        }

        public override Boolean ToBoolean() => new(Items.Length > 0);

        public override bool Equals(object obj) => ReferenceEquals(this, obj) || obj is Array array && Equals(array);

        public override int GetHashCode() => Items.GetHashCode();

        private bool Equals(Array other)
        {
            if (Items.Length != other.Items.Length)
                return false;

            return !Items.Where((item, idx) => !item.Equals(other.Items[idx])).Any();
        }
        
        private bool IsInBound(int index) => index >= 0 && index < Items.Length;

        private int NormalizeIndex(int index) => index >= 0 ? index : Items.Length + index;

        public static Array operator +(Array left, Array right)
        {
            var items = left.Items.Concat(right.Items);

            return new Array(items.ToArray());
        }

        public static Array operator *(Array left, Number right)
        {
            var amount = (int)Math.Round(right.Value);

            var items = Enumerable.Empty<Obj>();
            for (var i = 0; i < amount; i++)
                items = items.Concat(left.Items);

            return new Array(items.ToArray());
        }

        public static Boolean operator ==(Array left, Array right) => new(left.Equals(right));

        public static Boolean operator !=(Array left, Array right) => new(!left.Equals(right));
    }
}