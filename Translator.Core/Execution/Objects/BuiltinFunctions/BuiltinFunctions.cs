using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Translator.Core.Execution.Objects.BuiltinFunctions
{
    public static class BuiltinFunctions
    {
        public static readonly PrintFunction Print = new();
        public static readonly BoolFunction Bool = new();
        
        public static IEnumerable<Function> GetAll()
        {
            return typeof(BuiltinFunctions)
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(f => f.FieldType.IsSubclassOf(typeof(Function)))
                .Select(f => (Function)f.GetValue(null));
        }
    }
}