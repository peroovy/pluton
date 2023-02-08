using System;

namespace Core.Execution.DataModel.Objects.Functions
{
    public class Method : Function
    {
        public Method(Obj instance, Function function)
            : base($"{instance.TypeName}.{function.Name}",
                function.PositionParameters,
                function.DefaultParameters,
                context =>
                {
                    if (function.PositionParameters.Length == 0)
                        throw new InvalidOperationException("The function must contain more than one parameter");

                    context.Scope.Assign(function.PositionParameters[0], instance);

                    return function.Invoke(context);
                })
        {
        }

        public override string ToString()
        {
            return $"bound method <{Name}>";
        }
    }
}