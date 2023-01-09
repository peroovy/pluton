using System;
using Core.Execution.Objects;

namespace Core.Execution.Interrupts
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