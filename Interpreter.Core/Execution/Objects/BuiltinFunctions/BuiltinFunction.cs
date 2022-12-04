﻿using System;
using System.Collections.Immutable;

namespace Interpreter.Core.Execution.Objects.BuiltinFunctions
{
    public abstract class BuiltinFunction : Function
    {
        protected BuiltinFunction(
            string name, 
            ImmutableArray<string> positionParameters, 
            ImmutableArray<(string name, Obj value)> defaultParameters, 
            Action<Function, Scope, CallStack> call) : base(name, positionParameters, defaultParameters, call, isBuiltin: true)
        {
        }
    }
}