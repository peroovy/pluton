﻿using System;
using System.Collections.Immutable;

namespace Core.Execution.DataModel.Objects.Functions.Builtin
{
    public class StrFunction : BuiltinFunction
    {
        private const string ParameterName = "obj";

        public StrFunction(Lazy<IExecutor> executor)
            : base(
                "str",
                ImmutableArray.Create(ParameterName),
                ImmutableArray<CallArgument>.Empty,
                context => context.Arguments[ParameterName].ToStringObj(executor.Value))
        {
        }
    }
}