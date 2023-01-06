namespace Core.Utils.Text
{
    public class TextSpan
    {
        public TextSpan(int start, int length)
        {
            Start = start;
            Length = length;
        }
        
        public int Start { get; }
        
        public int Length { get; }

        public int End => Start + Length;
    }
    
    public class Location
    {
        public Location(SourceText sourceText, int start, int length)
        {
            SourceText = sourceText;
            Span = new TextSpan(start, length);
        }

        public Location(SourceText sourceText, TextSpan span)
        {
            SourceText = sourceText;
            Span = span;
        }
        
        public SourceText SourceText { get; }
        
        public TextSpan Span { get; }
    }
}