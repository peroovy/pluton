using Core.Lexing;

namespace Core.Diagnostic
{
    public class Log
    {
        public Log(Level level, string message, TextLocation location, int highlightCodeLength)
        {
            Level = level;
            Message = message;
            Location = location;
            HighlightCodeLength = highlightCodeLength;
        }
        
        public Level Level { get; }
        
        public string Message { get; }

        public TextLocation Location { get; }
        
        public int HighlightCodeLength { get; }
    }
}