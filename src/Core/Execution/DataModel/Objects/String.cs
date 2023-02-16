using System;
using System.Collections.Immutable;
using System.Text;
using Core.Execution.DataModel.Magic;
using Core.Execution.DataModel.Objects.Functions;
using Microsoft.CodeAnalysis.CSharp;

namespace Core.Execution.DataModel.Objects
{
    public class String : Obj, IIndexReadable
    {
        private static readonly Class BaseClass = new(nameof(String));

        private const string ObjParameter = "obj";

        static String()
        {
            BaseClass.SetAttribute(MagicFunctions.Len, new Function(
                MagicFunctions.Len,
                ImmutableArray.Create(ObjParameter),
                ImmutableArray<CallArgument>.Empty,
                context =>
                {
                    var str = (String)context.Arguments[ObjParameter];

                    return new Number(str.Value.Length);
                }
            ));
        }

        public String(string value) : base(BaseClass)
        {
            Value = value;
        }

        public string Value { get; }

        protected override bool IsTrue => Value.Length > 0;

        public override String ToReprObj(IExecutor executor)
        {
            return new String($"\"{SymbolDisplay.FormatLiteral(Value, false)}\"");
        }

        public Obj this[int index]
        {
            get
            {
                index = NormalizeIndex(index);

                if (!IsInBound(index))
                    throw new IndexOutOfRangeException();

                return new String(ToString()[index].ToString());
            }
        }

        public override string ToString()
        {
            return Value;
        }

        private bool IsInBound(int index)
        {
            return index >= 0 && index < Value.Length;
        }

        private int NormalizeIndex(int index)
        {
            return index >= 0 ? index : Value.Length + index;
        }
        
        public static String __add__(String left, String right)
        {
            return new String(left.Value + right.Value);
        }

        public static String __mult__(String str, Number number)
        {
            var result = new StringBuilder();

            var amount = (int)Math.Round(number.Value);
            for (var i = 0; i < amount; i++)
                result.Append(str.Value);

            return new String(result.ToString());
        }

        public static Bool __eq__(String left, String right)
        {
            return new Bool(left.Equals(right));
        }

        public static Bool __neq__(String left, String right)
        {
            return new Bool(!left.Equals(right));
        }

        public static Bool __lt__(String left, String right)
        {
            return new Bool(string.CompareOrdinal(left.Value, right.Value) < 0);
        }

        public static Bool __lte__(String left, String right)
        {
            return new Bool(string.CompareOrdinal(left.Value, right.Value) <= 0);
        }

        public static Bool __gt__(String left, String right)
        {
            return new Bool(string.CompareOrdinal(left.Value, right.Value) > 0);
        }

        public static Bool __gte__(String left, String right)
        {
            return new Bool(string.CompareOrdinal(left.Value, right.Value) >= 0);
        }
    }
}