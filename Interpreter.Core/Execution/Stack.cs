using System;
using System.Collections.Generic;
using Interpreter.Core.Execution.Objects;

namespace Interpreter.Core.Execution
{
    public class Stack
    {
        private readonly Stack<Obj> stack = new();

        public int Count => stack.Count;

        public void PushFunction(Function function) => stack.Push(function);
        
        public void PushFunctionResult(Obj value)
        {
            if (stack.Pop() is not Function)
                throw new InvalidOperationException("The top of the stack is not a function");
            
            stack.Push(value);
        }

        public Obj Peek() => stack.Peek();

        public Obj Pop() => stack.Pop();
    }
}