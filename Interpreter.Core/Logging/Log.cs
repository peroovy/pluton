using Interpreter.Core.Lexing;

namespace Interpreter.Core.Logging
{
    public class Log
    {
        public Log(Level level, string message, TextLocation location, int lengthError)
        {
            Level = level;
            Message = message;
            Location = location;
            LengthError = lengthError;
        }
        
        public Level Level { get; }
        
        public string Message { get; }
        
        public TextLocation Location { get; }
        
        public int LengthError { get; }
    }
}