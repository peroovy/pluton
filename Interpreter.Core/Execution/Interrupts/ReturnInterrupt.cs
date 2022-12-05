using System;
using Interpreter.Core.Execution.Objects;

namespace Interpreter.Core.Execution.Interrupts
{
    public class ReturnInterrupt : Exception
    {
        public ReturnInterrupt(Obj value)
        {
            Value = value;
        }
        
        public Obj Value { get; }
    }
}