using System;
using Core.Execution.DataModel.Objects;

namespace Core.Execution.Signals
{
    public class ReturnSignal : Exception
    {
        public ReturnSignal(Obj value)
        {
            Value = value;
        }
        
        public Obj Value { get; }
    }
}