using System;
using System.Collections.Immutable;

namespace Interpreter.Core.Execution.Objects.BuiltinFunctions
{
    public abstract class BuiltinFunction : Function
    {
        protected BuiltinFunction(
            string name, 
            ImmutableArray<string> positionParameters, 
            Action<Function, Scope, CallStack> call) : base(name, positionParameters, call, isBuiltin: true)
        {
        }
    }
}