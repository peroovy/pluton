using System;

namespace Core.Execution.DataModel.Objects.Functions
{
    public class MethodWrapper : Function
    {
        public MethodWrapper(Obj instance, Function function)
            : base($"{instance.TypeName}.{function.Name}",
                function.PositionParameters,
                function.DefaultParameters,
                context =>
                {
                    if (function.PositionParameters.Length == 0)
                        throw new InvalidOperationException("The function must contain more than one parameter");

                    var arguments = context
                        .Arguments
                        .SetItem(function.PositionParameters[0], instance);
                    context = new CallContext(context.Callable, arguments, context.Scope);

                    return function.Invoke(context);
                })
        {
        }

        public override string ToString()
        {
            return $"method <{Name}>";
        }
    }
}