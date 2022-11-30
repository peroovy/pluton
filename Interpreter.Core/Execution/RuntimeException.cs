using System;
using Interpreter.Core.Lexing;

namespace Interpreter.Core.Execution
{
    public class RuntimeException : Exception
    {
        public TextLocation Location { get; }

        public RuntimeException(TextLocation location)
        {
            Location = location;
        }
    }
}