using System;
using System.Collections.Immutable;
using Core.Execution.Objects.DataModel;

namespace Core.Execution.Objects
{
    public class Method : Obj, ICallable
    {
        private readonly Obj instance;
        private readonly Function function;

        public Method(Obj instance, Function function)
        {
            this.instance = instance;
            this.function = function;
            PositionParameters = function.PositionParameters;
            DefaultParameters = function.DefaultParameters;
            
            Invoke = context =>
            {
                if (PositionParameters.Length == 0)
                    throw new InvalidOperationException("The function must contain more than one parameter");

                context.Scope.Assign(PositionParameters[0], instance);

                return function.Invoke(context);
            };
        }

        public ImmutableArray<string> PositionParameters { get; }

        public ImmutableArray<CallArgument> DefaultParameters { get; }
        
        public Func<CallContext, Obj> Invoke { get; }

        public string Name => $"{instance.TypeName}.{function.Name}";

        public override string ToString() => $"bound method <{Name}>";
    }
}