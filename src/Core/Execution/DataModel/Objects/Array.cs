using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Core.Execution.DataModel.Magic;

namespace Core.Execution.DataModel.Objects
{
    public class Array : Obj, IIndexReadable, IIndexSettable, ICollection
    {
        private static readonly ClassObj BaseClassObj = new(nameof(Array));

        public Array(ImmutableArray<Obj> items) : base(BaseClassObj)
        {
            Items = items.ToArray();
        }

        private Array(Obj[] items) : base(BaseClassObj)
        {
            Items = items;
        }

        public int Length => Items.Length;

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

        public override string AsDebugString => ToString();

        public override string ToString()
        {
            var result = new StringBuilder();
            result.Append('[');

            for (var i = 0; i < Items.Length; i++)
            {
                result.Append(Items[i].AsDebugString);

                if (i + 1 < Items.Length)
                    result.Append(", ");
            }

            result.Append(']');
            return result.ToString();
        }

        private bool IsInBound(int index)
        {
            return index >= 0 && index < Items.Length;
        }

        private int NormalizeIndex(int index)
        {
            return index >= 0 ? index : Items.Length + index;
        }

        public static Array __add__(Array left, Array right)
        {
            var items = left.Items.Concat(right.Items);

            return new Array(items.ToArray());
        }

        public static Array __mult__(Array left, Number right)
        {
            var amount = (int)Math.Round(right.Value);

            var items = Enumerable.Empty<Obj>();
            for (var i = 0; i < amount; i++)
                items = items.Concat(left.Items);

            return new Array(items.ToArray());
        }

        public static Bool __eq__(Array left, Array right)
        {
            return new(Equals(left, right));
        }

        public static Bool __neq__(Array left, Array right)
        {
            return new(!Equals(left, right));
        }

        private static bool Equals(Array left, Array right)
        {
            if (ReferenceEquals(left, right))
                return true;

            if (left.Items.Length != right.Items.Length)
                return false;

            return !left.Items.Where((item, idx) => !item.Equals(right.Items[idx])).Any();
        }
    }
}