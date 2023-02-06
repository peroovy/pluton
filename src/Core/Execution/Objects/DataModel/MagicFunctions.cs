using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Core.Execution.Objects.DataModel
{
    public static class MagicFunctions
    {
        private static readonly HashSet<string> Names;

        public const string Initializer = "__init__";
        public const string New = "__new__";
        public const string Str = "__str__";
        public const string Repr = "__repr__";

        public const string EqualityOperation = "__eq__";
        public const string InequalityOperation = "__neq__";
        public const string LessThanOperation = "__lt__";
        public const string LessThanOrEqualOperation = "__lte__";
        public const string GreaterThanOperation = "__gt__";
        public const string GreaterThanOrEqualOperation = "__gte__";
        public const string LogicalAndOperation = "__and__";
        public const string LogicalOrOperation = "__or__";

        public const string AdditionOperation = "__add__";
        public const string SubtractionOperation = "__sub__";
        public const string MultiplicationOperation = "__mult__";
        public const string DivisionOperation = "__div__";
        public const string ModulusOperation = "__mod__";

        public const string UnaryPlusOperation = "__pos__";
        public const string UnaryNegativeOperation = "__neg__";
        public const string LogicalNotOperation = "__not__";

        static MagicFunctions()
        {
            Names = typeof(MagicFunctions)
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(field => field.IsLiteral && !field.IsInitOnly)
                .Select(field => (string)field.GetRawConstantValue())
                .ToHashSet();
        }

        public static bool IsMagic(string name) => Names.Contains(name);
    }
}